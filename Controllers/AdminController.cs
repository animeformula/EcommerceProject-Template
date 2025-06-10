using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using EcommerceProject.Models;

namespace EcommerceProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OrderManagement(string searchString, string statusFilter, string sortOrder, string dateFilter)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => 
                    (o.User != null && o.User.Email != null && o.User.Email.Contains(searchString)) || 
                    o.Id.ToString().Contains(searchString) ||
                    (o.ShippingAddress != null && o.ShippingAddress.Contains(searchString)));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse(statusFilter, out OrderStatus status))
            {
                orders = orders.Where(o => o.Status == status);
            }

            // Apply date filter
            if (!string.IsNullOrEmpty(dateFilter))
            {
                var today = DateTime.UtcNow.Date;
                switch (dateFilter)
                {
                    case "today":
                        orders = orders.Where(o => o.CreatedAt.Date == today);
                        break;
                    case "week":
                        orders = orders.Where(o => o.CreatedAt.Date >= today.AddDays(-7));
                        break;
                    case "month":
                        orders = orders.Where(o => o.CreatedAt.Date >= today.AddMonths(-1));
                        break;
                }
            }

            // Apply sorting
            orders = sortOrder switch
            {
                "date_desc" => orders.OrderByDescending(o => o.CreatedAt),
                "date_asc" => orders.OrderBy(o => o.CreatedAt),
                "amount_desc" => orders.OrderByDescending(o => o.TotalAmount),
                "amount_asc" => orders.OrderBy(o => o.TotalAmount),
                _ => orders.OrderByDescending(o => o.CreatedAt)
            };

            // Get order statistics
            ViewBag.TotalOrders = await orders.CountAsync();
            ViewBag.TotalRevenue = await orders.Where(o => o.Status != OrderStatus.Cancelled).SumAsync(o => o.TotalAmount);
            ViewBag.PendingOrders = await orders.CountAsync(o => o.Status == OrderStatus.Pending);
            ViewBag.ProcessingOrders = await orders.CountAsync(o => o.Status == OrderStatus.Processing);
            ViewBag.ShippedOrders = await orders.CountAsync(o => o.Status == OrderStatus.Shipped);
            ViewBag.DeliveredOrders = await orders.CountAsync(o => o.Status == OrderStatus.Delivered);

            return View(await orders.ToListAsync());
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            if (Enum.TryParse(status, out OrderStatus newStatus))
            {
                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            return RedirectToAction("OrderDetails", new { id });
        }

        public async Task<IActionResult> ExportOrders(string searchString, string statusFilter, string dateFilter)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Apply the same filters as in OrderManagement
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => 
                    (o.User != null && o.User.Email != null && o.User.Email.Contains(searchString)) || 
                    o.Id.ToString().Contains(searchString) ||
                    (o.ShippingAddress != null && o.ShippingAddress.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse(statusFilter, out OrderStatus status))
            {
                orders = orders.Where(o => o.Status == status);
            }

            if (!string.IsNullOrEmpty(dateFilter))
            {
                var today = DateTime.UtcNow.Date;
                switch (dateFilter)
                {
                    case "today":
                        orders = orders.Where(o => o.CreatedAt.Date == today);
                        break;
                    case "week":
                        orders = orders.Where(o => o.CreatedAt.Date >= today.AddDays(-7));
                        break;
                    case "month":
                        orders = orders.Where(o => o.CreatedAt.Date >= today.AddMonths(-1));
                        break;
                }
            }

            var orderList = await orders.ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Order ID,Customer Email,Status,Total Amount,Order Date,Shipping Address,City,State,Postal Code");

            foreach (var order in orderList)
            {
                csv.AppendLine($"{order.Id},{order.User?.Email ?? "N/A"},{order.Status},{order.TotalAmount},{order.CreatedAt},{order.ShippingAddress},{order.City},{order.State},{order.PostalCode}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"orders_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
} 