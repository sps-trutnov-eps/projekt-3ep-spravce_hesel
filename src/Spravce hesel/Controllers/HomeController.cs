using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Classes;
using Spravce_hesel.Data;
using Spravce_hesel.Models;

namespace Spravce_hesel.Controllers
{
    public class HomeController : Controller
    {
        private Spravce_heselData Databaze { get; set; }
        public HomeController(Spravce_heselData databaze)
        {
            Databaze = databaze;
        }

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

        [HttpGet]
        public IActionResult Oznameni()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    List<SdileneHeslo> hesla = Databaze.Sdilena_hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).Where(heslo => heslo.Potvrzeno == false).ToList();
                    return Ok(Json(hesla));
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }
    }
}