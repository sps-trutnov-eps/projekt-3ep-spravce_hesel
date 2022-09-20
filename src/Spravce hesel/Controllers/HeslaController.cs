using Microsoft.AspNetCore.Mvc;

namespace Spravce_hesel.Controllers
{
    public class HeslaController : Controller
    {
        public IActionResult Zobrazeni()
        {
            return View();
        }

        public IActionResult Pridat()
        {
            return View();
        }
    }
}
