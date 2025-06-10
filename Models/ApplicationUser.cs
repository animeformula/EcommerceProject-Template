using Microsoft.AspNetCore.Identity;

// This is the ApplicationUser model that inherits from IdentityUser
// It adds additional properties to the default IdentityUser class

namespace EcommerceProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 