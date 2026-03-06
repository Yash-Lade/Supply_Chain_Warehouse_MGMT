namespace SCM.API.DTOs.Vendor
{
    public class VendorCreateDto
    {
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int LeadTimeDays { get; set; }
        public bool IsPreferred { get; set; }
    }
}