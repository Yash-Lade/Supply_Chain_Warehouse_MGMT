namespace SCM_System.DTOs.Rol
{
    public class DraftPoDto
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = null!;
        public string VendorName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}