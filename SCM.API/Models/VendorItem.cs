namespace SCM.API.Models
{
    public class VendorItem
    {
        public int Id { get; set; }

        public int VendorId { get; set; }
        public int ItemId { get; set; }

        public decimal? LastPurchasePrice { get; set; }
        public decimal? ContractPrice { get; set; }

        public bool IsPreferred { get; set; }

        public DateTime LastUpdated { get; set; }

        public Vendor Vendor { get; set; }
        public Item Item { get; set; }
    }
}

