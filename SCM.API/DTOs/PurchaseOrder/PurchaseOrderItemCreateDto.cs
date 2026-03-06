namespace SCM.API.DTOs.PurchaseOrder
{
    public class PurchaseOrderItemCreateDto
    {
        public int ItemId { get; set; }
        public int OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}