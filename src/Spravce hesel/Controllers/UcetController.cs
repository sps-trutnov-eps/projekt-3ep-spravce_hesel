using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Spravce_hesel.Classes;
using Microsoft.EntityFrameworkCore;

namespace Spravce_hesel.Controllers
{
    public class UcetController : Controller
    {
        private SpravceHeselData Databaze { get; set; }
        public UcetController(SpravceHeselData databaze)
        {
            Databaze = databaze;
        }

        // Přihlášení
        [HttpGet]
        public IActionResult Prihlaseni()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId != null)
            {
                if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
                {
                    return RedirectToAction("Zobrazeni", "Hesla");
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Prihlaseni(Uzivatel obj)
        {
            ModelState.Clear();

            Uzivatel? prihlasujiciSeUzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Email == obj.Email);

            if (prihlasujiciSeUzivatel == null)
            {
                ModelState.AddModelError("Heslo", "E-Mail a heslo se neshodují");
            }
            else
            {
                if (BCrypt.Net.BCrypt.Verify(obj.Heslo, prihlasujiciSeUzivatel.Heslo) == false)
                {
                    ModelState.AddModelError("Heslo", "E-Mail a heslo se neshodují");
                }

                if (ModelState.IsValid && (HttpContext.Session.GetInt32("ID") == null || HttpContext.Session.GetString("Klic") == null))
                {
                    HttpContext.Session.SetInt32("ID", prihlasujiciSeUzivatel.Id);
                    HttpContext.Session.SetString("Klic", prihlasujiciSeUzivatel.Heslo);
                    return RedirectToAction("Zobrazeni", "Hesla");
                }
            }

            return View();

        }

        // Registrace
        [HttpGet]
        public IActionResult Registrace()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId != null)
            {
                if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
                {
                    return RedirectToAction("Zobrazeni", "Hesla");
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Registrace(Uzivatel obj, string? kontrolaHesla)
        {
            ModelState.Clear();

            IEnumerable<Uzivatel> objCategoryList = Databaze.Uzivatele;

            if (obj == null)
            {
                return StatusCode(500);
            }

            if (obj.Jmeno != null)
            {
                string jmeno = obj.Jmeno;

                if (jmeno.Contains(" "))
                {
                    ModelState.AddModelError("Jmeno", "Jméno nesmí obsahovat mezery.");
                }
            }
            else
            {
                ModelState.AddModelError("Jmeno", "Jméno nesmí obsahovat mezery.");
            }

            if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Email == obj.Email) != null)
            {
                ModelState.AddModelError("email", "Tento email už existuje");
            }

            if (kontrolaHesla == null || obj.Heslo != kontrolaHesla)
            {
                ModelState.AddModelError("Heslo", "Hesla se neshodují");
            }

            if (obj.Heslo == null || obj.Heslo.Length < 8)
            {
                ModelState.AddModelError("Heslo", "Heslo musí mít alespoň 8 znaků");
            }
            else 
            {
                obj.Heslo = BCrypt.Net.BCrypt.HashPassword(obj.Heslo);

                bool kontrola = true;
                Random randId = new();
                while (kontrola) {
                    obj.Id = randId.Next(1, 1000000);
                    kontrola = false;
                    foreach (var nah in objCategoryList)
                    {
                        if (nah.Id == obj.Id)
                        {
                            kontrola = true;
                        }
                    }
                }

                using (Aes aesAlg = Aes.Create())
                {
                    obj.IV = aesAlg.IV;
                }

                if (ModelState.IsValid && (HttpContext.Session.GetInt32("ID") == null || HttpContext.Session.GetString("Klic") == null))
                {
                    Databaze.Uzivatele.Add(obj);
                    Databaze.SaveChanges();

                    HttpContext.Session.SetInt32("ID", obj.Id);
                    HttpContext.Session.SetString("Klic", obj.Heslo);

                    return RedirectToAction("Zobrazeni", "Hesla");
                }
            }

            return View();
        }

        // Nastavení
        [HttpGet]
        public IActionResult Nastaveni()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == HttpContext.Session.GetInt32("ID"));
                if (uzivatel != null)
                {
                    ViewData["pocetHesel"] = Databaze.Hesla.Where(heslo => heslo.UzivatelskeId == uzivatelId).ToList().Count;
                    ViewData["pocetMnouSdilenychHesel"] = Databaze.SdilenaHesla.Where(heslo => heslo.ZakladatelId == uzivatelId).ToList().Count;
                    ViewData["pocetMneSdilenychHesel"] = Databaze.SdilenaHesla.Where(heslo => heslo.UzivatelskeId == uzivatelId).ToList().Count;
                    return View(uzivatel);
                }
            }

            return StatusCode(401);
        }

        // Změna jména
        [HttpGet]
        public IActionResult ZmenaJmena()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId != null)
            {
                if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
                {
                    return View();
                }
            }

            return StatusCode(401);
        }

        [HttpPost]
        public IActionResult ZmenaJmena(string novejmeno, Uzivatel obj)
        {
            ModelState.Clear();

            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId == null)
            {
                return StatusCode(401);
            }

            Uzivatel? prihlasenyUzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);

            if (prihlasenyUzivatel != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(obj.Heslo, prihlasenyUzivatel.Heslo))
                {
                    ModelState.AddModelError("Heslo", "Špatné heslo");
                }
            }
            else
            {
                return StatusCode(401);
            }

            if (ModelState.IsValid)
            {
                prihlasenyUzivatel.Jmeno = novejmeno;

                Databaze.Entry(prihlasenyUzivatel).State = EntityState.Modified;

                Databaze.SdilenaHesla.Where(heslo => heslo.ZakladatelId == prihlasenyUzivatel.Id).ToList()
                    .ForEach(heslo => heslo.ZakladatelJmeno = prihlasenyUzivatel.Jmeno + " (" + prihlasenyUzivatel.Email + ")");

                Databaze.SaveChanges();

                HttpContext.Session.SetInt32("ID", prihlasenyUzivatel.Id);
                HttpContext.Session.SetString("Klic", prihlasenyUzivatel.Heslo);

                return RedirectToAction("Zobrazeni", "Hesla");
            }

            return View();
        }

        // Změna hesla
        [HttpGet]
        public IActionResult ZmenaHesla()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId != null)
            {
                if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
                {
                    return View();
                }
            }

            return StatusCode(401);
        }

        [HttpPost]
        public IActionResult ZmenaHesla(string noveheslo, string noveheslokontrola, Uzivatel obj)
        {
            ModelState.Clear();

            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? uzivatelKlic = HttpContext.Session.GetString("Klic");
            if (uzivatelId == null || uzivatelKlic == null)
            {
                return StatusCode(401);
            }

            Uzivatel? prihlasenyUzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);

            if (prihlasenyUzivatel != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(obj.Heslo, prihlasenyUzivatel.Heslo))
                {
                    ModelState.AddModelError("Heslo", "Špatné heslo");
                }
            }
            else
            {
                return StatusCode(500);
            }

            if (noveheslo != noveheslokontrola)
            {
                ModelState.AddModelError("Heslo", "Hesla se neshodují");
            }

            if (noveheslo != null && noveheslo.Length > 7)
            {
                prihlasenyUzivatel.Heslo = BCrypt.Net.BCrypt.HashPassword(noveheslo);
            }

            var puvodniIV = prihlasenyUzivatel.IV;

            if (ModelState.IsValid)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    prihlasenyUzivatel.IV = aesAlg.IV;
                }

                Databaze.Entry(prihlasenyUzivatel).State = EntityState.Modified;
                Databaze.SaveChanges();

                byte[] klic = Sifrovani.HesloNaKlic(uzivatelKlic);
                byte[] klicNovy = Sifrovani.HesloNaKlic(prihlasenyUzivatel.Heslo);

                Databaze.Hesla.Where(heslo => heslo.UzivatelskeId == prihlasenyUzivatel.Id).ToList()
                    .ForEach(heslo => heslo.Sifra = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Sifra, klic, puvodniIV), klicNovy, prihlasenyUzivatel.IV));

                Databaze.SaveChanges();
                
                HttpContext.Session.SetInt32("ID", prihlasenyUzivatel.Id);
                HttpContext.Session.SetString("Klic", prihlasenyUzivatel.Heslo);

                return RedirectToAction("Zobrazeni", "Hesla");
            }

            return View();
        }

        // Odebrání účtu
        [HttpGet]
        public IActionResult Odebrani()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId != null)
            {
                if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
                {
                    return View();
                }
            }

            return StatusCode(401);
        }

        [HttpPost]
        public IActionResult Odebrani(string heslo)
        {
            ModelState.Clear();

            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId == null)
            {
                return StatusCode(401);
            }

            Uzivatel? prihlasenyUzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);

            if (prihlasenyUzivatel != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(heslo, prihlasenyUzivatel.Heslo))
                {
                    ModelState.AddModelError("Heslo", "Špatné heslo");
                }
            }
            else
            {
                return StatusCode(500);
            }

            if (ModelState.IsValid)
            {
                Databaze.Hesla.RemoveRange(Databaze.Hesla.Where(hesloDatabaze => hesloDatabaze.UzivatelskeId == uzivatelId).ToList());
                Databaze.SdilenaHesla.RemoveRange(Databaze.SdilenaHesla.Where(hesloDatabaze => hesloDatabaze.ZakladatelId == uzivatelId).ToList());
                Databaze.Uzivatele.Remove(prihlasenyUzivatel);

                Databaze.SaveChanges();

                HttpContext.Session.Remove("ID");
                HttpContext.Session.Remove("Klic");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Odhlášení
        [HttpGet]
        public IActionResult Odhlaseni()
        {
            HttpContext.Session.Remove("ID");
            HttpContext.Session.Remove("Klic");
            return RedirectToAction("Index", "Home");
        }
    }
}
