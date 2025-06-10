using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EcommerceProject.ViewModels
{
    public class CategoryCreateViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }
} 