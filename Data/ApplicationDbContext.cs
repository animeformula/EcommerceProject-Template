using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Models;

// This is the ApplicationDbContext class that inherits from IdentityDbContext<ApplicationUser>
// It defines the database context for the application

namespace EcommerceProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // This is the constructor for the ApplicationDbContext class
        // It takes a DbContextOptions<ApplicationDbContext> object as a parameter
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // This is the DbSet for the Product model
        public DbSet<Product> Products { get; set; }

        // This is the DbSet for the Category model
        public DbSet<Category> Categories { get; set; }

        // This is the DbSet for the Order model
        public DbSet<Order> Orders { get; set; }

        // This is the DbSet for the OrderItem model
        public DbSet<OrderItem> OrderItems { get; set; }

        // This is the DbSet for the CartItem model
        public DbSet<CartItem> CartItems { get; set; }

        // This is the DbSet for the Wishlist model
        public DbSet<Wishlist> Wishlists { get; set; }

        // This is the DbSet for the Review model
        public DbSet<Review> Reviews { get; set; }

        // This is the OnModelCreating method that configures the relationships and constraints for the models
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // This is the relationship between the OrderItem and Order models
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // This is the relationship between the OrderItem and Product models
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for CartItems
            builder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 