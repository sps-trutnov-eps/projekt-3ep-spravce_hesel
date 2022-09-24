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
            if (prihlasujiciseuzivatel != null && heslo != null && (HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Klic") == null))
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
        public IActionResult Registrace(uzivatel obj, string kontrola_hesla)
        {
            if (obj.Username == null ||obj.Username.Length < 2)
            {
                obj.Username = "Lmao";
                ModelState.AddModelError("username", "◀ Jméno je příliš krátké.");
            }
           
            if (Databaze.uzivatel.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault() != null)
            {
                ModelState.AddModelError("email", "◀ Tento email už existuje.");
            }

            if (obj.Email == null)
            {
                ModelState.AddModelError("email", "◀ Zadejte email.");
            }

            if (obj.Heslo != kontrola_hesla)
            {
                ModelState.AddModelError("heslo", "◀ Hesla se neshodují.");
            }

            if (obj.Heslo != null && obj.Heslo.Length > 7)
            {
                obj.Heslo = BCrypt.Net.BCrypt.HashPassword(obj.Heslo);
            }
            else {
                ModelState.AddModelError("heslo", "◀ Heslo musí mít 8 znaků a více.");
            }

            if (ModelState.IsValid && (HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Klic") == null))
            {
                Databaze.uzivatel.Add(obj);
                Databaze.SaveChanges();

                HttpContext.Session.SetString("Email", obj.Email);
                HttpContext.Session.SetString("Klic", obj.Heslo);

                return RedirectToAction("Zobrazeni", "Hesla");
            }
            return View();
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
