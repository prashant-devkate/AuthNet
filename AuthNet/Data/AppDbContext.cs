using AuthNet.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PurchaseOrder> purchaseOrders { get; set; }
        public DbSet<AuditLog> auditLogs { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<CompanyInfo> CompanyInfos { get; set; }
        public DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }
        public DbSet<CompanyInfoHistory> CompanyInfoHistories { get; set; }

        public DbSet<TaxSetting> TaxSettings { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<PurchaseOrderItems> PurchaseOrderItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);           

            // Seed: Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Electronics", Description = "Electronic items" },
                new Category { CategoryId = 2, Name = "Books", Description = "Books and Stationery" }
            );

            // Seed: Suppliers
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { SupplierId = 1, CompanyName = "Tech Supplies Inc", ContactName = "Alice", Phone = "111-111", Address = "NYC" },
                new Supplier { SupplierId = 2, CompanyName = "BookMart", ContactName = "Bob", Phone = "222-222", Address = "LA" }
            );

            // Seed: Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "admin", PasswordHash = "hashed-password", Role = "Admin", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<Inventory>().HasIndex(i => i.ProductId).IsUnique();

        }
    }
}
