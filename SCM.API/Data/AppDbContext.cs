using Microsoft.EntityFrameworkCore;
using SCM.API.Models;
using SCM_System.Models.Workflow;

namespace SCM.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Auth
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Master
        public DbSet<Item> Items { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        // Inventory
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockLedger> StockLedger { get; set; }

        // Purchase Order
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        // Approval 
        public DbSet<Approval> Approvals { get; set; }

        // Goods Receipt Note - GRN
        public DbSet<GRN> GRNs { get; set; }
        public DbSet<GRNItem> GRNItems { get; set; }

        // Vendor Items for ROL :-)
        public DbSet<VendorItem> VendorItems { get; set; }
        // for telling the schema structure
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("Roles", "Auth");
            modelBuilder.Entity<User>().ToTable("Users", "Auth");

            modelBuilder.Entity<Item>().ToTable("Items", "Master");
            modelBuilder.Entity<Warehouse>().ToTable("Warehouses", "Master");
            modelBuilder.Entity<Vendor>().ToTable("Vendors", "Master");

            modelBuilder.Entity<Batch>().ToTable("Batches", "Inventory");
            modelBuilder.Entity<Stock>().ToTable("Stock", "Inventory");
            modelBuilder.Entity<StockLedger>().ToTable("StockLedger", "Inventory");

            modelBuilder.Entity<PurchaseOrder>().ToTable("PurchaseOrders", "Procurement");
            modelBuilder.Entity<PurchaseOrderItem>().ToTable("PurchaseOrderItems", "Procurement");

            modelBuilder.Entity<Approval>().ToTable("Approvals", "workflow");

            modelBuilder.Entity<GRN>().ToTable("GRNs", "Procurement");
            modelBuilder.Entity<GRNItem>().ToTable("GRNItems", "Procurement");

            modelBuilder.Entity<VendorItem>().ToTable("VendorItems", "Master");

            modelBuilder.Entity<VendorItem>()
                .HasOne(v => v.Vendor)
                .WithMany(v => v.VendorItems)
                .HasForeignKey(v => v.VendorId);

            modelBuilder.Entity<VendorItem>()
                .HasOne(v => v.Item)
                .WithMany(i => i.VendorItems)
                .HasForeignKey(v => v.ItemId);
        }
    }
}