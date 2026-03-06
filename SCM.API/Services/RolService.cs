using Microsoft.EntityFrameworkCore;
using SCM.API.Data;
using SCM.API.Models;

namespace SCM_System.Services
{
    public class RolService
    {
        private readonly AppDbContext _context;

        public RolService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetCurrentStockAsync(int itemId)
        {
            var totalStock = await _context.Stock
                .Include(s => s.Batch)
                .Where(s => s.Batch.ItemId == itemId)
                .SumAsync(s => (decimal?)s.Quantity) ?? 0;

            return totalStock;
        }
        public async Task<decimal> CalculateRolAsync(int itemId, int vendorId)
        {
            var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);

            // 1️⃣ Calculate total OUT quantity
            var totalOut = await _context.StockLedger
                .Where(l =>
                    l.ItemId == itemId &&
                    l.TransactionType == "OUT" &&
                    l.TransactionDate >= ninetyDaysAgo)
                .SumAsync(l => (decimal?)l.Quantity) ?? 0;

            decimal dailyUsage = totalOut / 90;

            // 2️⃣ Get vendor lead time
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null)
                throw new Exception("Vendor not found");

            int leadTime = vendor.LeadTimeDays;

            // 3️⃣ Safety Stock (basic buffer)
            decimal safetyStock = dailyUsage * 7;

            decimal rol = (dailyUsage * leadTime) + safetyStock;

            return Math.Ceiling(rol);
        }
        public async Task CheckAndGeneratePoAsync()
        {
            var items = await _context.Items
                .Where(i => i.IsActive)
                .ToListAsync();

            foreach (var item in items)
            {
                var vendorItem = await _context.VendorItems
                .Where(v => v.ItemId == item.Id && v.IsPreferred)
                .FirstOrDefaultAsync();

                if (vendorItem == null)
                continue;

                var rol = await CalculateRolAsync(item.Id, vendorItem.VendorId);
                var currentStock = await GetCurrentStockAsync(item.Id);

                if (currentStock <= rol)
                {
                    var poExists = await _context.PurchaseOrders
                    .AnyAsync(p =>
                    p.Status == "Draft" &&
                    p.VendorId == vendorItem.VendorId &&
                    p.Items.Any(i => i.ItemId == item.Id));

                    if (poExists)
                    continue;

                    var po = new PurchaseOrder
                    {
                    PONumber = $"AUTO-{DateTime.UtcNow.Ticks}",
                    VendorId = vendorItem.VendorId,
                    WarehouseId = 1, // adjust properly later
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1, // system user
                    TotalAmount = 0
                    };

                    _context.PurchaseOrders.Add(po);
                    await _context.SaveChangesAsync();

                    var poItem = new PurchaseOrderItem
                    {
                    PurchaseOrderId = po.Id,
                    ItemId = item.Id,
                    OrderedQuantity = item.MaxStockLevel - (int)currentStock,
                    UnitPrice = vendorItem.LastPurchasePrice ?? 0,
                    ReceivedQuantity = 0
                    };

                    _context.PurchaseOrderItems.Add(poItem);

                    po.TotalAmount = poItem.OrderedQuantity * poItem.UnitPrice;

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}   