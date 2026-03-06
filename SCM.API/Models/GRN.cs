using SCM.API.Models;

public class GRN
{
    public int Id { get; set; }

    public int PurchaseOrderId { get; set; }

    public string GRNNumber { get; set; } = null!;

    public string Status { get; set; } = "Completed"; // Completed / Partial

    public DateTime ReceivedDate { get; set; }

    public int ReceivedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PurchaseOrder PurchaseOrder { get; set; }
}