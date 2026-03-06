namespace SCM.API.DTOs.Item
{
    public class ItemCreateDto
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public string UnitType { get; set; }
        public string ABCClass { get; set; }
        public string XYZClass { get; set; }
        public int MinStockLevel { get; set; }
        public int MaxStockLevel { get; set; }
    }
}