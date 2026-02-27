using Microsoft.AspNetCore.Mvc;
using SCM.API.Data;
using SCM_System.DTOs;
using SCM_System.Models.Workflow;
using System.Security.Claims;

namespace SCM_System.Controllers
{
    [ApiController]
    [Route("api/purchaseorders")]
    public class ApprovalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApprovalsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPut("{poId}/approve")]
        public IActionResult TakeAction(int poId, [FromBody] ApprovalActionDto dto)
        {
            var po = _context.PurchaseOrders.Find(poId);

            if (po == null)
                return NotFound("PO not found");

            if (po.Status != "PendingApproval")
                return BadRequest("PO not submitted for approval");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var roleId = int.Parse(User.FindFirstValue(ClaimTypes.Role));

            int level = roleId switch
            {
                2 => 1,  // HOD
                3 => 2,  // Finance
                4 => 3,  // Director
                _ => 0
            };

            if (level == 0)
                return Forbid("Not authorized for approval");

            var approval = _context.Approvals.FirstOrDefault(a =>
                a.ReferenceType == "PurchaseOrder" &&
                a.ReferenceId == poId &&
                a.ApprovalLevel == level);

            if (approval == null)
                return BadRequest("Approval record not found");

            if (approval.Status != "Pending")
                return BadRequest("Already processed");

            // Enforce sequential hierarchy
            var previousApproved = _context.Approvals
                .Where(a =>
                    a.ReferenceType == "PurchaseOrder" &&
                    a.ReferenceId == poId &&
                    a.ApprovalLevel < level)
                .All(a => a.Status == "Approved");

            if (!previousApproved)
                return BadRequest("Previous level not approved");

            //var normalized = dto.Action.Equals("Approved", StringComparison.OrdinalIgnoreCase)
            //    ? "Approved"
            //    : dto.Action.Equals("Rejected", StringComparison.OrdinalIgnoreCase)
            //        ? "Rejected"
            //        : null;

            var actionValue = dto.Action?.Trim().ToLower();

            string? normalized = actionValue switch
            {
                "approved" => "Approved",
                "approve" => "Approved",
                "rejected" => "Rejected",
                "reject" => "Rejected",
                _ => null
            };

            if (normalized == null)
                return BadRequest("Invalid action");

            approval.Status = normalized;
            approval.ApprovedBy = userId;
            approval.ActionTime = DateTime.UtcNow;
            approval.Remarks = dto.Remarks;

            _context.SaveChanges();

            if (normalized == "Rejected")
            {
                po.Status = "Rejected";
            }
            else
            {
                var allApproved = _context.Approvals
                    .Where(a => a.ReferenceType == "PurchaseOrder" && a.ReferenceId == poId)
                    .All(a => a.Status == "Approved");

                if (allApproved)
                    po.Status = "Approved";
            }

            _context.SaveChanges();

            return Ok(new
            {
                Level = level,
                Status = normalized
            });
        }
    }
}