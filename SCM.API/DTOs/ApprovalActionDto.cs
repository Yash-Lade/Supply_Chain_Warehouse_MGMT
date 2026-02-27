namespace SCM_System.DTOs
{
    public class ApprovalActionDto
    {
        public string Action { get; set; } = null!;  // Approved / Rejected
        public string? Remarks { get; set; }
    }
}