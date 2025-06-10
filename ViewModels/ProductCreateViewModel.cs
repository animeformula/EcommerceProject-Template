using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EcommerceProject.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }
} 