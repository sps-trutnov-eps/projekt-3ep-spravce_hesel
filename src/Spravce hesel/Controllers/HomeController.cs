using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Models;
using System.Diagnostics;

namespace Spravce_hesel.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ZmenitBarevneTema()
        {
            if (HttpContext.Session.GetString("Tema") == null || HttpContext.Session.GetString("Tema") == "light")
                HttpContext.Session.SetString("Tema", "dark");
            else
                HttpContext.Session.SetString("Tema", "light");

            return RedirectToAction("Index");
        }
    }
}