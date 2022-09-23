using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;

namespace Spravce_hesel.Controllers
{
    public class UcetController : Controller
    {
        private Spravce_heselData Databaze { get; set; }
        public UcetController(Spravce_heselData databaze)
        {
            Databaze = databaze;
        }

        // Přihlášení
        [HttpGet]
        public IActionResult Prihlaseni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Prihlaseni(string email, string heslo)
        {
            uzivatel? prihlasujiciseuzivatel = Databaze.uzivatel.Where(uzivatel => uzivatel.Email == email).FirstOrDefault();
            if (prihlasujiciseuzivatel != null && heslo != null)
            {
                if (BCrypt.Net.BCrypt.Verify(heslo, prihlasujiciseuzivatel.Heslo))
                {
                    HttpContext.Session.SetString("Email", email);
                    HttpContext.Session.SetString("Klic", heslo);
                    return RedirectToAction("Zobrazeni", "Hesla");
                }
            }

            return RedirectToAction("Prihlaseni");

        }

        // Registrace
        [HttpGet]
        public IActionResult Registrace()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrace(string jmeno, string heslo, string kontrola_hesla, string email)
        {
            uzivatel? existujici = Databaze.uzivatel.Where(uzivatel => uzivatel.Email == email).FirstOrDefault();

            if (heslo == null || jmeno == null || kontrola_hesla == null || email == null || heslo != kontrola_hesla || jmeno == heslo || email == heslo || existujici != null)
            {
                return RedirectToAction("Registrace", "Ucet");
            }

            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetString("klic", heslo);


            heslo = BCrypt.Net.BCrypt.HashPassword(heslo);
            Databaze.uzivatel.Add(new uzivatel() { Email = email, Heslo = heslo, Username = jmeno });
            Databaze.SaveChanges();

            return RedirectToAction("Zobrazeni", "Hesla");
        }

        // Nastavení
        [HttpGet]
        public IActionResult Nastaveni()
        {
            return View();
        }

        // Změna jména
        [HttpGet]
        public IActionResult ZmenaJmena()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ZmenaJmena(string novejmeno, string heslo)
        {
            return RedirectToAction("Nastaveni");
        }

        // Změna hesla
        [HttpGet]
        public IActionResult ZmenaHesla()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ZmenaHesla(string noveheslo, string heslo)
        {
            return RedirectToAction("Nastaveni");
        }

        // Odebrání účtu
        [HttpGet]
        public IActionResult Odebrani()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Odebrani(string heslo)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
