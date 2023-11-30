using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TattooStudioBooking.Models;

namespace TattooStudioBooking.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BookingContext _context;
    public HomeController(ILogger<HomeController> logger, BookingContext context)
    {
        _logger = logger;
        this._context = context;
    }

    [Authorize]
    public IActionResult Index()
    {        
        return View(_context.PuestosTrabajo.ToList());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public IActionResult MyClaims()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
