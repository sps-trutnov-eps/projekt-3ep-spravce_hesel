using Microsoft.AspNetCore.Mvc;

namespace Spravce_hesel.Controllers
{
    public class NapovedaController : Controller
    {
        [Route("/Napoveda/{sekce=Uvod}")]
        [Route("/Napoveda/Index/{sekce=Uvod}")]
        [Route("/Pomoc/{sekce=Uvod}")]
        [Route("/Help/{sekce=Uvod}")]
        public IActionResult Index(string sekce)
        {
            ViewData["Sekce"] = sekce;
            return View();
        }
    }
}
