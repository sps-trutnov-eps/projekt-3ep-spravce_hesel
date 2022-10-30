using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Classes;
using Spravce_hesel.Data;
using Spravce_hesel.Models;

namespace Spravce_hesel.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Home/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            ViewData["Chyba"] = $"{code}";
            return View();
        }
    }
}