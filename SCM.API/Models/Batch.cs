namespace SCM.API.Models
{
    public class Batch
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Item Item { get; set; }
    }
}