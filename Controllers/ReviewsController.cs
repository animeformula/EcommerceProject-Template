using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Models;
using EcommerceProject.Data;
using System.Security.Claims;

namespace EcommerceProject.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int productId, int rating, string comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                var review = new Review
                {
                    ProductId = productId,
                    UserId = userId,
                    Rating = rating,
                    Comment = comment
                };
                _context.Reviews.Add(review);
            }

            await _context.SaveChangesAsync();

            var product = await _context.Products
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return Json(new
            {
                averageRating = product.AverageRating,
                reviewCount = product.ReviewCount,
                reviews = product.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        userName = r.User.UserName,
                        rating = r.Rating,
                        comment = r.Comment,
                        createdAt = r.CreatedAt
                    })
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    userName = r.User.UserName,
                    rating = r.Rating,
                    comment = r.Comment,
                    createdAt = r.CreatedAt
                })
                .ToListAsync();

            return Json(reviews);
        }
    }
} 