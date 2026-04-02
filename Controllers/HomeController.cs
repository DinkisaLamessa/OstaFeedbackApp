<<<<<<< HEAD
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OstaFeedbackApp.Models;

namespace OstaFeedbackApp.Controllers;

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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
=======
using Microsoft.AspNetCore.Mvc;

namespace OstaFeedbackApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Landing page
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
>>>>>>> a47f374 (Clean repo without large files)
