using System.ComponentModel.DataAnnotations;

// This is the Category model that represents a category in the database
// It has properties for the category's ID, name, description, image URL, and a collection of products  

namespace EcommerceProject.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Product> Products { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
} 