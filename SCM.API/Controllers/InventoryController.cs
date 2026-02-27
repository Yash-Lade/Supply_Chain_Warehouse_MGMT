using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.API.Data;
using SCM.API.Models;

namespace SCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add-stock")]
        public IActionResult AddStock(int itemId, int warehouseId, string batchNumber, DateTime expiryDate, int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than zero");

            // Validate Item
            var itemExists = _context.Items.Any(i => i.Id == itemId);
            if (!itemExists)
                return BadRequest("Invalid ItemId");

            // Validate Warehouse
            var warehouseExists = _context.Warehouses.Any(w => w.Id == warehouseId);
            if (!warehouseExists)
                return BadRequest("Invalid WarehouseId");
            
            // Check if batch exists
            var batch = _context.Batches
                .FirstOrDefault(b => b.ItemId == itemId && b.BatchNumber == batchNumber);

            if (batch == null)
            {
                batch = new Batch
                {
                    ItemId = itemId,
                    BatchNumber = batchNumber,
                    ExpiryDate = expiryDate,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Batches.Add(batch);
                _context.SaveChanges();
            }

            // Check stock entry
            var stock = _context.Stock
                .FirstOrDefault(s => s.BatchId == batch.Id && s.WarehouseId == warehouseId);

            if (stock == null)
            {
                stock = new Stock
                {
                    BatchId = batch.Id,
                    WarehouseId = warehouseId,
                    Quantity = quantity,
                    LastUpdated = DateTime.UtcNow
                };
                _context.Stock.Add(stock);
            }
            else
            {
                stock.Quantity += quantity;
                stock.LastUpdated = DateTime.UtcNow;
            }

            // Add ledger entry
            var ledger = new StockLedger
            {
                ItemId = itemId,
                BatchId = batch.Id,
                WarehouseId = warehouseId,
                TransactionType = "IN",
                Quantity = quantity,
                TransactionDate = DateTime.UtcNow,
                CreatedBy = 1
            };

            _context.StockLedger.Add(ledger);
            _context.SaveChanges();

            return Ok("Stock added successfully");
        }

        // Deduct Stock Controller 
        [HttpPost("deduct-stock")]
        public IActionResult DeductStock(int itemId, int warehouseId, string batchNumber, int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than zero");

            // Validate item
            var itemExists = _context.Items.Any(i => i.Id == itemId);
            if (!itemExists)
                return BadRequest("Invalid ItemId");

            // Validate warehouse
            var warehouseExists = _context.Warehouses.Any(w => w.Id == warehouseId);
            if (!warehouseExists)
                return BadRequest("Invalid WarehouseId");

            // Find batch
            var batch = _context.Batches
                .FirstOrDefault(b => b.ItemId == itemId && b.BatchNumber == batchNumber);

            if (batch == null)
                return BadRequest("Batch not found");

            // Find stock record
            var stock = _context.Stock
                .FirstOrDefault(s => s.BatchId == batch.Id && s.WarehouseId == warehouseId);

            if (stock == null || stock.Quantity < quantity)
                return BadRequest("Insufficient stock");

            // Deduct quantity
            stock.Quantity -= quantity;
            stock.LastUpdated = DateTime.UtcNow;

            // Add ledger entry
            var ledger = new StockLedger
            {
                ItemId = itemId,
                BatchId = batch.Id,
                WarehouseId = warehouseId,
                TransactionType = "OUT",
                Quantity = quantity,
                TransactionDate = DateTime.UtcNow,
                CreatedBy = 1
            };

            _context.StockLedger.Add(ledger);
            _context.SaveChanges();

            return Ok("Stock deducted successfully");
        }

        // Transfer stock between warehouses 
        [HttpPost("transfer-stock")]
        public IActionResult TransferStock(
            int itemId,
            int sourceWarehouseId,
            int destinationWarehouseId,
            string batchNumber,
            int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than zero");

            if (sourceWarehouseId == destinationWarehouseId)
                return BadRequest("Source and destination cannot be same");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Validate item
                if (!_context.Items.Any(i => i.Id == itemId))
                    return BadRequest("Invalid ItemId");

                // Validate warehouses
                if (!_context.Warehouses.Any(w => w.Id == sourceWarehouseId))
                    return BadRequest("Invalid Source Warehouse");

                if (!_context.Warehouses.Any(w => w.Id == destinationWarehouseId))
                    return BadRequest("Invalid Destination Warehouse");

                // Find batch
                var batch = _context.Batches
                    .FirstOrDefault(b => b.ItemId == itemId && b.BatchNumber == batchNumber);

                if (batch == null)
                    return BadRequest("Batch not found");

                // Source stock
                var sourceStock = _context.Stock
                    .FirstOrDefault(s => s.BatchId == batch.Id && s.WarehouseId == sourceWarehouseId);

                if (sourceStock == null || sourceStock.Quantity < quantity)
                    return BadRequest("Insufficient stock in source warehouse");

                // Deduct from source
                sourceStock.Quantity -= quantity;
                sourceStock.LastUpdated = DateTime.UtcNow;

                // Destination stock
                var destinationStock = _context.Stock
                    .FirstOrDefault(s => s.BatchId == batch.Id && s.WarehouseId == destinationWarehouseId);

                if (destinationStock == null)
                {
                    destinationStock = new Stock
                    {
                        BatchId = batch.Id,
                        WarehouseId = destinationWarehouseId,
                        Quantity = quantity,
                        LastUpdated = DateTime.UtcNow
                    };
                    _context.Stock.Add(destinationStock);
                }
                else
                {
                    destinationStock.Quantity += quantity;
                    destinationStock.LastUpdated = DateTime.UtcNow;
                }

                // Ledger - OUT
                _context.StockLedger.Add(new StockLedger
                {
                    ItemId = itemId,
                    BatchId = batch.Id,
                    WarehouseId = sourceWarehouseId,
                    TransactionType = "TRANSFER",
                    Quantity = quantity,
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = 1
                });

                // Ledger - IN
                _context.StockLedger.Add(new StockLedger
                {
                    ItemId = itemId,
                    BatchId = batch.Id,
                    WarehouseId = destinationWarehouseId,
                    TransactionType = "TRANSFER",
                    Quantity = quantity,
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = 1
                });

                _context.SaveChanges();
                transaction.Commit();

                return Ok("Stock transferred successfully");
            }
            catch
            {
                transaction.Rollback();
                return StatusCode(500, "Transfer failed");
            }
        }
    }
    
}