using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCM.API.Data;
using SCM_System.DTOs.Rol;
using SCM_System.Services;

namespace SCM_System.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/rol")]
    public class RolController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RolService _rolService;
          
        public RolController(AppDbContext context,RolService rolService)
        {
            _context = context;
            _rolService = rolService;
        }

        [HttpGet("{itemId}/{vendorId}")]
        public async Task<IActionResult> GetRol(int itemId, int vendorId)
        {
            var rol = await _rolService.CalculateRolAsync(itemId, vendorId);

            return Ok(new
            {
                ItemId = itemId,
                Rol = rol
            });
        }

        // temproary for test
        //[HttpPost("run")]
        //public async Task<IActionResult> RunRol()
        //{
        //    await _rolService.CheckAndGeneratePoAsync();
        //    return Ok("ROL executed");
        //}

        // ANALYTICS
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var items = await _context.Items
                .Where(i => i.IsActive)
                .ToListAsync();

            var result = new List<RolAnalyticsDto>();

            foreach (var item in items)
            {
                var currentStock = await _context.Stock
                    .Include(s => s.Batch)
                    .Where(s => s.Batch.ItemId == item.Id)
                    .SumAsync(s => (decimal?)s.Quantity) ?? 0;

                var vendorItem = await _context.VendorItems
                    .Where(v => v.ItemId == item.Id && v.IsPreferred)
                    .FirstOrDefaultAsync();

                decimal rol = 0;

                if (vendorItem != null)
                {
                    rol = await _rolService.CalculateRolAsync(item.Id, vendorItem.VendorId);
                }

                result.Add(new RolAnalyticsDto
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    CurrentStock = currentStock,
                    Rol = rol
                });
            }

            return Ok(result);
        }

        // ITEMS BELOW ROL
        [HttpGet("below")]
        public async Task<IActionResult> GetBelowRol()
        {
            var analytics = await GetAnalyticsInternal();

            var below = analytics
                .Where(x => x.CurrentStock <= x.Rol)
                .ToList();

            return Ok(below);
        }

        // AUTO-GENERATED DRAFT POs
        [HttpGet("draft-pos")]
        public IActionResult GetDraftPOs()
        {
            var draftPOs = _context.PurchaseOrders
                .Where(p => p.Status == "Draft" && p.PONumber.StartsWith("AUTO-"))
                .Select(p => new DraftPoDto
                {
                    Id = p.Id,
                    PONumber = p.PONumber,
                    VendorName = p.Vendor.Name,
                    CreatedAt = p.CreatedAt
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return Ok(draftPOs);
        }

        // INTERNAL HELPER
        private async Task<List<RolAnalyticsDto>> GetAnalyticsInternal()
        {
            var items = await _context.Items
                .Where(i => i.IsActive)
                .ToListAsync();

            var result = new List<RolAnalyticsDto>();

            foreach (var item in items)
            {
                var currentStock = await _context.Stock
                    .Include(s => s.Batch)
                    .Where(s => s.Batch.ItemId == item.Id)
                    .SumAsync(s => (decimal?)s.Quantity) ?? 0;

                var vendorItem = await _context.VendorItems
                    .Where(v => v.ItemId == item.Id && v.IsPreferred)
                    .FirstOrDefaultAsync();

                decimal rol = 0;

                if (vendorItem != null)
                {
                    rol = await _rolService.CalculateRolAsync(item.Id, vendorItem.VendorId);
                }

                result.Add(new RolAnalyticsDto
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    CurrentStock = currentStock,
                    Rol = rol
                });
            }

            return result;
        }
    }
}