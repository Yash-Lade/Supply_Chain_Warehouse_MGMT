namespace SCM_System.Models.Workflow
{
    public class Approval
    {
        public int Id { get; set; }

        public string ReferenceType { get; set; } = null!;
        public int ReferenceId { get; set; }

        public int ApprovalLevel { get; set; }  // 1, 2, 3

        public int? ApprovedBy { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime? SLADeadline { get; set; }

        public DateTime? ActionTime { get; set; }

        public string? Remarks { get; set; }
    }
}