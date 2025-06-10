using Microsoft.AspNetCore.Identity;
using EcommerceProject.Models;

namespace EcommerceProject.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles if they don't exist
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    Address = "123 Admin St",
                    City = "Admin City",
                    State = "Admin State",
                    PostalCode = "12345",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create categories if they don't exist
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Electronics",
                        Description = "Latest electronic gadgets and devices",
                        ImageUrl = "/images/categories/electronics.jpeg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Clothing",
                        Description = "Fashionable clothing for all seasons",
                        ImageUrl = "/images/categories/clothing.jpeg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Books",
                        Description = "Best-selling books and literature",
                        ImageUrl = "/images/categories/books.jpeg",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Create products if they don't exist
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Smartphone X",
                        Description = "Latest smartphone with advanced features",
                        Price = 999.99m,
                        StockQuantity = 50,
                        ImageUrl = "/images/products/smartphone.jpeg",
                        CategoryId = context.Categories.First(c => c.Name == "Electronics").Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Laptop Pro",
                        Description = "High-performance laptop for professionals",
                        Price = 1499.99m,
                        StockQuantity = 30,
                        ImageUrl = "/images/products/laptop.jpeg",
                        CategoryId = context.Categories.First(c => c.Name == "Electronics").Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Designer T-Shirt",
                        Description = "Comfortable and stylish t-shirt",
                        Price = 29.99m,
                        StockQuantity = 100,
                        ImageUrl = "/images/products/tshirt.jpeg",
                        CategoryId = context.Categories.First(c => c.Name == "Clothing").Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Bestselling Novel",
                        Description = "Award-winning fiction novel",
                        Price = 19.99m,
                        StockQuantity = 200,
                        ImageUrl = "/images/products/book.jpeg",
                        CategoryId = context.Categories.First(c => c.Name == "Books").Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
} 