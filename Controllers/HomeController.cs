using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inventory_Management.Models;

namespace Inventory_Management.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult NotFound(int statusCode)
    {
        if(statusCode == 404)
        {
            return View("NotFound");
        }
        else
        {
            return View("Error");
        }
    }

    public IActionResult ServerError(int statusCode)
    {
        if (statusCode == 500)
        {
            return View("ServerError");
        }
        else
        {
            return View("Error");
        }
    }
}