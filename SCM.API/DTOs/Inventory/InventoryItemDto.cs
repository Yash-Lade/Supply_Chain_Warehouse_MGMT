namespace SCM_System.DTOs.Inventory
{
    public class InventoryItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;
        public decimal CurrentStock { get; set; }
        public decimal Rol { get; set; }
        public decimal CriticalLevel { get; set; }
    }
}