using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCM.API.Data;
using SCM.API.Models;

[ApiController]
[Route("api/grn")]
public class GRNController : ControllerBase
{
    private readonly AppDbContext _context;

    public GRNController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateGRN([FromBody] CreateGrnDto dto)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var po = _context.PurchaseOrders
                .Include(p => p.Items)
                .FirstOrDefault(p => p.PONumber == dto.PONumber);

            if (po == null)
                return NotFound("PO not found");

            if (po.Status != "Approved")
                return BadRequest("GRN allowed only for Approved PO");

            var existingGrn = _context.GRNs
                .Any(g => g.PurchaseOrderId == po.Id);

            if (existingGrn)
                return BadRequest("GRN already created");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var grn = new GRN
            {
                PurchaseOrderId = po.Id,
                GRNNumber = $"GRN-{DateTime.UtcNow.Ticks}",
                Status = "Completed",
                ReceivedDate = DateTime.UtcNow,
                ReceivedBy = userId
            };

            _context.GRNs.Add(grn);

            foreach (var item in po.Items)
            {
                int receivedQty = item.OrderedQuantity;

                // Create batch
                var batch = new Batch
                {
                    ItemId = item.ItemId,
                    BatchNumber = $"BATCH-{Guid.NewGuid().ToString().Substring(0, 8)}",
                    ExpiryDate = DateTime.UtcNow.AddYears(1), // adjust later
                    CreatedAt = DateTime.UtcNow
                };

                _context.Batches.Add(batch);
                _context.SaveChanges(); // to generate Batch.Id

                // Update stock
                var stock = _context.Stock
                    .FirstOrDefault(s =>
                        s.BatchId == batch.Id &&
                        s.WarehouseId == po.WarehouseId);

                if (stock == null)
                {
                    stock = new Stock
                    {
                        WarehouseId = po.WarehouseId,
                        BatchId = batch.Id,
                        Quantity = receivedQty,
                        LastUpdated = DateTime.UtcNow
                    };

                    _context.Stock.Add(stock);
                }
                else
                {
                    stock.Quantity += receivedQty;
                    stock.LastUpdated = DateTime.UtcNow;
                }

                // Insert ledger entry
                var ledger = new StockLedger
                {
                    ItemId = item.ItemId,
                    BatchId = batch.Id,
                    WarehouseId = po.WarehouseId,
                    TransactionType = "IN",
                    Quantity = receivedQty,
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.StockLedger.Add(ledger);

                // Update PO item received qty
                item.ReceivedQuantity = receivedQty;
            }

            po.Status = "Received";

            _context.SaveChanges();

            transaction.Commit();

            return Ok("GRN received successfully");
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
