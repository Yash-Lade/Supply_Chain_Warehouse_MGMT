namespace SCM.API.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string UnitType { get; set; }
        public string ABCClass { get; set; }
        public string XYZClass { get; set; }
        public int MinStockLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<VendorItem> VendorItems { get; set; }
    }
}