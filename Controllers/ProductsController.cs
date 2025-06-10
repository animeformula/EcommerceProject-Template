using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EcommerceProject.Data;
using EcommerceProject.Models;
using EcommerceProject.ViewModels;

// This is the ProductsController class that handles the products in the application

namespace EcommerceProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // This is the Index action that displays the products
        public async Task<IActionResult> Index(int? categoryId, string searchString, string sortOrder)
        {
            // This is the query to get the products
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsQueryable();

            // This is the filter to get the products by category
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            // This is the search to get the products by name or description
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            // This is the sort to get the products by name or price
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParam"] = sortOrder == "price" ? "price_desc" : "price";

            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                _ => products.OrderBy(p => p.Name),
            };

            // Get categories for filter dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchString = searchString;
            ViewBag.SortOrder = sortOrder;

            var productList = await products.ToListAsync();
            var userId = User.Identity.IsAuthenticated ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null;
            var wishlists = userId != null ? await _context.Wishlists.Where(w => w.UserId == userId).ToListAsync() : new List<Wishlist>();
            ViewBag.UserWishlist = wishlists.Select(w => w.ProductId).ToHashSet();

            return View(productList);
        }

        // This is the Details action that displays the product details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var userId = User.Identity.IsAuthenticated ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null;
            var wishlists = userId != null ? await _context.Wishlists.Where(w => w.UserId == userId).ToListAsync() : new List<Wishlist>();
            ViewBag.UserWishlist = wishlists.Select(w => w.ProductId).ToHashSet();
            return View(product);
        }

        // This is the Create action that displays the create product view
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View(new ProductCreateViewModel());
        }

        // This is the Create action that handles the creation of a new product
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            ViewBag.Categories = _context.Categories.ToList();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Handle file upload
            string imageUrl = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
                imageUrl = "/images/products/" + uniqueFileName;
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Please upload a product image.");
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                CategoryId = model.CategoryId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // This is the Edit action that displays the edit product view
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // This is the Edit action that handles the editing of a product
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,StockQuantity,CategoryId,IsActive,ImageUrl")] Product product, IFormFile imageFile)
        {
            if (id != product.Id)
                return NotFound();

            // Remove validation for navigation/collection properties
            ModelState.Remove("CartItems");
            ModelState.Remove("OrderItems");
            ModelState.Remove("Reviews");
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }

            var originalProduct = await _context.Products.FindAsync(id);
            if (originalProduct == null)
                return NotFound();

            // Update fields
            originalProduct.Name = product.Name;
            originalProduct.Description = product.Description;
            originalProduct.Price = product.Price;
            originalProduct.StockQuantity = product.StockQuantity;
            originalProduct.CategoryId = product.CategoryId;
            originalProduct.IsActive = product.IsActive;

            // Handle image
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + "_" + imageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Optionally delete old image if local
                if (!string.IsNullOrEmpty(originalProduct.ImageUrl) && originalProduct.ImageUrl.StartsWith("/images/products/"))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, originalProduct.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                originalProduct.ImageUrl = "/images/products/" + uniqueFileName;
            }
            else
            {
                // Use the value from the text field (can be remote URL or left unchanged)
                originalProduct.ImageUrl = product.ImageUrl;
            }

            originalProduct.UpdatedAt = DateTime.UtcNow;

            _context.Update(originalProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // This is the Delete action that displays the delete product view
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            // This is the check to see if the product id is null
            if (id == null)
            {
                return NotFound();
            }

            // This is the query to get the product
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // This is the Delete action that handles the deletion of a product
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // This is the query to get the product
            var product = await _context.Products.FindAsync(id);

            // This is the check to see if the product is null
            if (product != null)
            {
                // This is the query to remove the product from the database
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
} 