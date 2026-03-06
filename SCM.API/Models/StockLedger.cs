namespace SCM.API.Models
{
    public class StockLedger
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int BatchId { get; set; }
        public int WarehouseId { get; set; }
        public string TransactionType { get; set; } // IN, OUT, TRANSFER
        public string ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public int CreatedBy { get; set; }
    }
}