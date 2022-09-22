using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Models;
using System.Diagnostics;

namespace Spravce_hesel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}