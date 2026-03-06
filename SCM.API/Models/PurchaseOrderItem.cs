namespace SCM.API.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ItemId { get; set; }
        public int OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReceivedQuantity { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }
        public Item Item { get; set; }
    }
}