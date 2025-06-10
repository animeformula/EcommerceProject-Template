using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Data;
using EcommerceProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceProject.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show checkout page
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartItems = await _context.CartItems.Include(ci => ci.Product).Where(ci => ci.UserId == userId).ToListAsync();
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");
            ViewBag.CartItems = cartItems;
            ViewBag.Total = cartItems.Sum(ci => ci.TotalPrice);
            return View();
        }

        // Handle order placement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string shippingAddress, string city, string state, string postalCode)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartItems = await _context.CartItems.Include(ci => ci.Product).Where(ci => ci.UserId == userId).ToListAsync();
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                City = city,
                State = state,
                PostalCode = postalCode,
                TotalAmount = cartItems.Sum(ci => ci.TotalPrice),
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };
            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }
    }
} 