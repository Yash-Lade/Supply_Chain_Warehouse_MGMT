namespace SCM_System.DTOs.Rol
{
    public class RolAnalyticsDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public decimal CurrentStock { get; set; }
        public decimal Rol { get; set; }
    }
}