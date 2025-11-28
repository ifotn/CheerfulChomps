using System.Diagnostics;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheerfulChomps.Controllers
{
    public class HomeController : Controller
    {
        // commented lesson 12 - not using logger feature
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            // specify view name for clarity
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
