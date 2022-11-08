using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

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
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
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

            Uzivatel? prihlasujiciseuzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault();

            if (prihlasujiciseuzivatel == null)
            {
                ModelState.AddModelError("Heslo", "E-Mail a heslo se neshodují");
            }
            else
            {
                if (BCrypt.Net.BCrypt.Verify(obj.Heslo, prihlasujiciseuzivatel.Heslo) == false)
                {
                    ModelState.AddModelError("Heslo", "E-Mail a heslo se neshodují");
                }

                if (ModelState.IsValid && (HttpContext.Session.GetInt32("ID") == null || HttpContext.Session.GetString("Klic") == null))
                {
                    HttpContext.Session.SetInt32("ID", prihlasujiciseuzivatel.Id);
                    HttpContext.Session.SetString("Klic", prihlasujiciseuzivatel.Heslo);
                    return RedirectToAction("Zobrazeni", "Hesla");
                }
            }

            return View();

        }

        // Registrace
        [HttpGet]
        public IActionResult Registrace()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    return RedirectToAction("Zobrazeni", "Hesla");
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Registrace(Uzivatel obj, string? kontrola_hesla)
        {
            ModelState.Clear();

            IEnumerable<Uzivatel> objCategoryList = Databaze.Uzivatele;

            if (obj == null)
            {
                return StatusCode(500);
            }

            obj.Jmeno = obj.Jmeno.Trim();

            if (obj.Jmeno.Length < 4 || obj.Jmeno == null)
            {
                ModelState.AddModelError("Jmeno", "Jméno nesmí obsahovat mezery.");
            }

            if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault() != null)
            {
                ModelState.AddModelError("email", "Tento email už existuje");
            }

            if (kontrola_hesla == null || obj.Heslo != kontrola_hesla)
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
                Random randID = new Random();
                while (kontrola) {
                    obj.Id = randID.Next(1, 1000000);
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
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == HttpContext.Session.GetInt32("ID")).FirstOrDefault();
                if (uzivatel != null)
                {
                    ViewData["Pocethesel"] = Databaze.Hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).ToList().Count;
                    return View(uzivatel);
                }
            }

            return StatusCode(401);
        }

        // Změna jména
        [HttpGet]
        public IActionResult ZmenaJmena()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
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

            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID == null)
            {
                return StatusCode(401);
            }

            Uzivatel? prihlaseny_uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();

            if (prihlaseny_uzivatel != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(obj.Heslo, prihlaseny_uzivatel.Heslo))
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
                obj.Email = prihlaseny_uzivatel.Email;
                obj.Jmeno = novejmeno;
                obj.Id = prihlaseny_uzivatel.Id;
                obj.Heslo = prihlaseny_uzivatel.Heslo;
                Databaze.Uzivatele.Remove(prihlaseny_uzivatel);
                Databaze.Uzivatele.Add(obj);
                Databaze.SaveChanges();

                HttpContext.Session.SetInt32("ID", obj.Id);
                HttpContext.Session.SetString("Klic", obj.Heslo);

                return RedirectToAction("Zobrazeni", "Hesla");
            }

            return View();
        }

        // Změna hesla
        [HttpGet]
        public IActionResult ZmenaHesla()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
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

            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID == null)
            {
                return StatusCode(401);
            }

            Uzivatel? prihlaseny_uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();

            if (prihlaseny_uzivatel != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(obj.Heslo, prihlaseny_uzivatel.Heslo))
                {
                    ModelState.AddModelError("Heslo", "Špatné heslo");
                }
            }
            else
            {
                StatusCode(500);
            }

            if (noveheslo != noveheslokontrola)
            {
                ModelState.AddModelError("Heslo", "Hesla se neshodují");
            }

            if (noveheslo != null && noveheslo.Length > 7)
            {
                obj.Heslo = BCrypt.Net.BCrypt.HashPassword(noveheslo);
            }

            if (ModelState.IsValid)
            {
                obj.Email = prihlaseny_uzivatel.Email;
                obj.Jmeno = prihlaseny_uzivatel.Jmeno;
                obj.Id = prihlaseny_uzivatel.Id;
                Databaze.Uzivatele.Remove(prihlaseny_uzivatel);
                Databaze.Uzivatele.Add(obj);
                Databaze.SaveChanges();

                HttpContext.Session.SetInt32("ID", obj.Id);
                HttpContext.Session.SetString("Klic", obj.Heslo);

                return RedirectToAction("Zobrazeni", "Hesla");
            }

            return View();
        }

        // Odebrání účtu
        [HttpGet]
        public IActionResult Odebrani()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
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

            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID == null)
            {
                return StatusCode(401);
            }

            Uzivatel? prihlaseny_uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();

            if (prihlaseny_uzivatel != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(heslo, prihlaseny_uzivatel.Heslo))
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
                List<Heslo> hesla = Databaze.Hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).ToList();
                if (hesla != null)
                {
                    foreach (Heslo h in hesla)
                    {
                        Databaze.Hesla.Remove(h);
                    }
                }

                Databaze.Uzivatele.Remove(prihlaseny_uzivatel);
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
