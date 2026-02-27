namespace SCM.API.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; }
        public int VendorId { get; set; }
        public int WarehouseId { get; set; }
        public string Status { get; set; } // Draft, PendingApproval, Approved, Closed
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Vendor Vendor { get; set; }
        public Warehouse Warehouse { get; set; }
        public ICollection<PurchaseOrderItem> Items { get; set; }
    }
}