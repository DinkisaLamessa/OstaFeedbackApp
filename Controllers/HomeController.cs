using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OstaFeedbackApp.Models;

namespace OstaFeedbackApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // =============================
        // LANDING PAGE
        // =============================
        public IActionResult Index()
        {
            return View();
        }

        // =============================
        // ABOUT PAGE (NEW)
        // =============================
        public IActionResult About()
        {
            return View();
        }

        // =============================
        // PRIVACY
        // =============================
        public IActionResult Privacy()
        {
            return View();
        }

        // =============================
        // ERROR HANDLING
        // =============================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
