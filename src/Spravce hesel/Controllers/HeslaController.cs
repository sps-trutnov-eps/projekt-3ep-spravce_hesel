using Microsoft.AspNetCore.Mvc;

namespace Spravce_hesel.Controllers
{
    public class HeslaController : Controller
    {
        public IActionResult Zobrazeni()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pridat()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Pridat(string sluzba, string jmeno, string heslo)
        {
            return RedirectToAction("Zobrazeni");
        }
    }
}
