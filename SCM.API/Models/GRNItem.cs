namespace SCM.API.Models
{
    public class GRNItem
    {
        public int Id { get; set; }
        public int GRNId { get; set; }
        public int PurchaseOrderItemId { get; set; }

        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }

        public int ReceivedQuantity { get; set; }

        public GRN GRN { get; set; }
        public PurchaseOrderItem PurchaseOrderItem { get; set; }
    }
}