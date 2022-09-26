using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.ComponentModel.DataAnnotations;

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

            IEnumerable<uzivatel> objCategoryList = Databaze.uzivatel;

            if (Databaze.uzivatel.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault() != null)
            {
                ModelState.AddModelError("email", "◀ Tento email už existuje.");
            }

            if (obj.Heslo != kontrola_hesla)
            {
                ModelState.AddModelError("heslo", "◀ Hesla se neshodují.");
            }

            foreach (var nah in objCategoryList)
            {
                if (BCrypt.Net.BCrypt.Verify(obj.Heslo, nah.Heslo))
                {
                    ModelState.AddModelError("heslo", "◀ Toto heslo používá už uživatel " + nah.Username + ", zvolte prosím jiné heslo.");
                }
            }

            if (obj.Heslo != null && obj.Heslo.Length > 7)
            {
                obj.Heslo = BCrypt.Net.BCrypt.HashPassword(obj.Heslo);
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
            string? email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }
            uzivatel? prihlaseny_uzivatel = Databaze.uzivatel.Where(uzivatel => uzivatel.Email == email).FirstOrDefault();
            if (prihlaseny_uzivatel != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(heslo, prihlaseny_uzivatel.Heslo))
                {
                    //misto pro vraceni chyby
                }
            }


            List<heslo> hesla = Databaze.heslo.Where(heslo => heslo.Email == email).ToList();
            if (hesla == null)
            {
                foreach (heslo h in hesla)
                {
                    Databaze.heslo.Remove(h);
                }
            }
            
            Databaze.uzivatel.Remove(prihlaseny_uzivatel);


            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");

        }
    }
}
