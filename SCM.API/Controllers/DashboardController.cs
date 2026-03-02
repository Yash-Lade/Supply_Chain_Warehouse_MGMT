using Microsoft.AspNetCore.Mvc;
using SCM.API.Data;

namespace SCM_System.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            var totalPOs = _context.PurchaseOrders.Count();

            var pendingApprovals = _context.PurchaseOrders
                .Count(p => p.Status == "PendingApproval");

            var approvedPOs = _context.PurchaseOrders
                .Count(p => p.Status == "Approved");

            var receivedPOs = _context.PurchaseOrders
                .Count(p => p.Status == "Received");

            var lowStockItems = _context.Stock
                .Count(s => s.Quantity < 50); // temporary threshold

            return Ok(new
            {
                totalPOs,
                pendingApprovals,
                approvedPOs,
                receivedPOs,
                lowStockItems
            });
        }
    }
}