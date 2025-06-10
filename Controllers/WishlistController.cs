using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using EcommerceProject.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EcommerceProject.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlist = _context.Products
                .Where(p => _context.Wishlists.Any(w => w.UserId == userId && w.ProductId == p.Id))
                .ToList();
            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (!_context.Wishlists.Any(w => w.UserId == userId && w.ProductId == productId))
            {
                _context.Wishlists.Add(new Wishlist { UserId = userId, ProductId = productId });
                await _context.SaveChangesAsync();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var item = _context.Wishlists.FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);
            if (item != null)
            {
                _context.Wishlists.Remove(item);
                await _context.SaveChangesAsync();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
} 