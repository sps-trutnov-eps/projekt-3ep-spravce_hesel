using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Models;
using System.Diagnostics;

namespace Spravce_hesel.Controllers
{
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
    }
}