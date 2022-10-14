using Microsoft.AspNetCore.Mvc;

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