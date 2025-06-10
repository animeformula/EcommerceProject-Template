using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using EcommerceProject.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Id)
            .Take(2)
            .ToListAsync();
        ViewBag.FeaturedProducts = featuredProducts;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
