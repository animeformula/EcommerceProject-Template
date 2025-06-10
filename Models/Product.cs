using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

// This is the Product model that represents a product in the database
// It has properties for the product's ID, name, description, price, image URL, stock quantity, category ID, and timestamps for creation and update

namespace EcommerceProject.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int StockQuantity { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Review> Reviews { get; set; }

        [NotMapped]
        public double AverageRating => (Reviews != null && Reviews.Any()) ? Reviews.Average(r => r.Rating) : 0;

        [NotMapped]
        public int ReviewCount => Reviews?.Count ?? 0;
    }
} 