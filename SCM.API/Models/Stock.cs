namespace SCM.API.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int BatchId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }

        public Warehouse Warehouse { get; set; }
        public Batch Batch { get; set; }
    }
}