using Inventory_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Models.Order> Orders { get; set; }
        public DbSet<Models.Category> Categories { get; set; }
        public DbSet<Models.OrderProduct> OrdersProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Orders)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Products)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Categories)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // // Seed data for Category
            // modelBuilder.Entity<Category>().HasData(
            //     new Category { CategoryId = 1, CategoryName = "Electronics", CategoryDescription = "Electronic items" },
            //     new Category { CategoryId = 2, CategoryName = "Clothing", CategoryDescription = "Apparel and garments" },
            //     new Category { CategoryId = 3, CategoryName = "Groceries", CategoryDescription = "Daily groceries and food items" }
            // );
            //
            // // Seed data for Product
            // modelBuilder.Entity<Product>().HasData(
            //     new Product { ProductId = 1, ProductName = "Lenovo Laptop", ProductDescription = "A high-performance laptop", ProductPrice = 999.99M, ProductQuantity = 50, LowStockThreshold = 10, CategoryId = 1 },
            //     new Product { ProductId = 2, ProductName = "T-Shirt", ProductDescription = "A comfortable cotton t-shirt", ProductPrice = 19.99M, ProductQuantity = 200, LowStockThreshold = 20, CategoryId = 2 },
            //     new Product { ProductId = 3, ProductName = "Apple", ProductDescription = "A fresh red apple", ProductPrice = 0.99M, ProductQuantity = 500, LowStockThreshold = 50, CategoryId = 3 }
            // );
            //
            // // Seed data for Order
            // modelBuilder.Entity<Order>().HasData(
            //     new Order { OrderId = 1, OrderDate = DateTime.Now, TotalPrice = 1019.98M, GuestName = "John Doe", GuestEmail = "john.doe@example.com" },
            //     new Order { OrderId = 2, OrderDate = DateTime.Now, TotalPrice = 20.98M, GuestName = "Jane Smith", GuestEmail = "jane.smith@example.com" }
            // );
            //
            // // Seed data for OrderProduct
            // modelBuilder.Entity<OrderProduct>().HasData(
            //     new OrderProduct { OrderId = 1, ProductId = 1, Quantity = 1 },
            //     new OrderProduct { OrderId = 1, ProductId = 2, Quantity = 1 },
            //     new OrderProduct { OrderId = 2, ProductId = 2, Quantity = 1 },
            //     new OrderProduct { OrderId = 2, ProductId = 3, Quantity = 1 }
            // );
        }
    }
}
