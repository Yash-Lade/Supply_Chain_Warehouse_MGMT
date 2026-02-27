using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCM.API.Data;
using SCM.API.DTOs.PurchaseOrder;
using SCM.API.Models;
using SCM_System.Models.Workflow;

namespace SCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseOrdersController(AppDbContext context)
        {
            _context = context;
        }
  
        // CREATE PURCHASE ORDER
        

        [HttpPost]
        public IActionResult CreatePO(PurchaseOrderCreateDto dto)
        {
            if (!_context.Vendors.Any(v => v.Id == dto.VendorId))
                return BadRequest("Invalid Vendor");

            if (!_context.Warehouses.Any(w => w.Id == dto.WarehouseId))
                return BadRequest("Invalid Warehouse");

            var po = new PurchaseOrder
            {
                PONumber = "PO-" + DateTime.UtcNow.Ticks,
                VendorId = dto.VendorId,
                WarehouseId = dto.WarehouseId,
                Status = "Draft",
                TotalAmount = 0,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.PurchaseOrders.Add(po);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPO), new { poId = po.Id }, po);
        }

        // ADD ITEM TO PURCHASE ORDER

        [HttpPost("{poId}/items")]
        public IActionResult AddItem(int poId, PurchaseOrderItemCreateDto dto)
        {
            var po = _context.PurchaseOrders.Find(poId);
            if (po == null)
                return NotFound("PO not found");

            if (po.Status != "Draft")
                return BadRequest("Only Draft PO can be modified");

            if (!_context.Items.Any(i => i.Id == dto.ItemId))
                return BadRequest("Invalid Item");

            if (dto.OrderedQuantity <= 0 || dto.UnitPrice <= 0)
                return BadRequest("Invalid quantity or price");

            var poItem = new PurchaseOrderItem
            {
                PurchaseOrderId = poId,
                ItemId = dto.ItemId,
                OrderedQuantity = dto.OrderedQuantity,
                UnitPrice = dto.UnitPrice,
                ReceivedQuantity = 0
            };

            _context.PurchaseOrderItems.Add(poItem);

            po.TotalAmount += dto.OrderedQuantity * dto.UnitPrice;

            _context.SaveChanges();

            return Ok("Item added successfully");
        }

        // GET PURCHASE ORDER WITH ITEMS

        [HttpGet("{poId}")]
        public IActionResult GetPO(int poId)
        {
            var po = _context.PurchaseOrders
                .Include(p => p.Items)
                .FirstOrDefault(p => p.Id == poId);

            if (po == null)
                return NotFound("PO not found");

            var result = new
            {
                po.Id,
                po.PONumber,
                po.VendorId,
                po.WarehouseId,
                po.Status,
                po.TotalAmount,
                po.CreatedAt,
                Items = po.Items.Select(i => new
                {
                    i.Id,
                    i.ItemId,
                    i.OrderedQuantity,
                    i.UnitPrice,
                    i.ReceivedQuantity
                })
            };

            return Ok(result);
        }

        // purchase order approvals
        [HttpPost("{poId}/submit")]
        public IActionResult SubmitForApproval(int poId)
        {
            var po = _context.PurchaseOrders.Find(poId);

            if (po == null)
                return NotFound("PO not found");

            if (po.Status != "Draft")
                return BadRequest("Only Draft PO can be submitted");

            var alreadySubmitted = _context.Approvals
                .Any(a => a.ReferenceType == "PurchaseOrder" && a.ReferenceId == poId);

            if (alreadySubmitted)
                return BadRequest("Already submitted");

            var approvals = new List<Approval>
            {
                new Approval { ReferenceType = "PurchaseOrder", ReferenceId = poId, ApprovalLevel = 1 },
                new Approval { ReferenceType = "PurchaseOrder", ReferenceId = poId, ApprovalLevel = 2 },
                new Approval { ReferenceType = "PurchaseOrder", ReferenceId = poId, ApprovalLevel = 3 }
            };

            _context.Approvals.AddRange(approvals);

            po.Status = "PendingApproval";

            _context.SaveChanges();

            return Ok("Submitted for approval");
        }
        // UPDATE PO STATUS (PUT)

        //[HttpPut("{poId}/status")]
        //public IActionResult UpdateStatus(int poId, PurchaseOrderStatusUpdateDto dto)
        //{
        //    var po = _context.PurchaseOrders.Find(poId);
        //    if (po == null)
        //        return NotFound("PO not found");

        //    var validStatuses = new[]
        //    {
        //        "Draft",
        //        "PendingApproval",
        //        "Approved",
        //        "Rejected",
        //        "Closed"
        //    };

        //    var normalizedStatus = validStatuses
        //        .FirstOrDefault(s => s.Equals(dto.Status, StringComparison.OrdinalIgnoreCase));

        //    if (normalizedStatus == null)
        //        return BadRequest("Invalid status");

        //    po.Status = normalizedStatus;
        //    _context.SaveChanges();

        //    return Ok("Status updated successfully");
        //}
    }
}