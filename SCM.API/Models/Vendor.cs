namespace SCM.API.Models
{
    
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int LeadTimeDays { get; set; }
        public bool IsPreferred { get; set; }
        public decimal PerformanceScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<VendorItem> VendorItems { get; set; } // FOR Cleaner implementation
    }
}