using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.ComponentModel.DataAnnotations;
using Spravce_hesel.Classes;
using System.Diagnostics;

namespace Spravce_hesel.Controllers
{
    
    public class HeslaController : Controller
    {
        private Spravce_heselData Databaze { get; set; }
        public HeslaController(Spravce_heselData databaze)
        {
            Databaze = databaze;
        }

        [HttpGet]
        public IActionResult Zobrazeni()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                klic = Sifrovani.HesloNaKlic(klic);
                List<Heslo> Hesla = Databaze.Hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).ToList();
                List<Heslo> desifrovano = new List<Heslo>();

                foreach(Heslo heslo in Hesla)
                {
                    Heslo _desforave = new Heslo() {
                        ID = heslo.ID,
                        UzivatelskeID = heslo.UzivatelskeID,
                        Sluzba = heslo.Sluzba,
                        Jmeno = heslo.Jmeno,
                        Hash = heslo.Hash,
                        Sifra = Sifrovani.Desifrovat(klic, heslo.Sifra)
                    };

                    Debug.Print(_desforave.Sifra);
                    Debug.Print(Sifrovani.Desifrovat(klic, heslo.Sifra));

                    desifrovano.Add(_desforave);
                }

                List<SdileneHeslo> hesla = Databaze.Sdilena_hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).Where(heslo => heslo.Potvrzeno == false).ToList();
                ViewData["Oznameni"] = hesla;

                return View(desifrovano);
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet] 
        public IActionResult DetailHesla(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            Heslo? heslo = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
            if (uzivatelID != null && klic != null && heslo != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null
                && heslo.UzivatelskeID == uzivatelID)
            {
                klic = Sifrovani.HesloNaKlic(klic);
                heslo.Sifra = Sifrovani.Desifrovat(klic, heslo.Sifra);
                return Ok(Json(heslo));
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        public IActionResult Pridat()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                return View();
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult Pridat(string sluzba, string jmeno, string heslo)
        {
            ModelState.Clear();

            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null)
                {
                    if (heslo == null || heslo.Length == 0)
                    {
                        ModelState.AddModelError("Jmeno", "Vyplňte toto pole!");
                    }
                    if (ModelState.IsValid)
                    {
                        int delka = klic.Length;

                        klic = Sifrovani.HesloNaKlic(klic);

                        int hash = heslo.GetHashCode();
                        heslo = Sifrovani.Zasifrovat(klic, heslo);

                        Heslo h = new()
                        {
                            UzivatelskeID = (int)uzivatelID,
                            Sluzba = sluzba,
                            Jmeno = jmeno,
                            Hash = hash,
                            Sifra = heslo
                        };

                        Databaze.Hesla.Add(h);
                        Databaze.SaveChanges();

                        return RedirectToAction("Zobrazeni");
                    }

                    return View();
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult Odstranit(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                Heslo? heslo = Databaze.Hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).Where(heslo => heslo.ID == id).FirstOrDefault();
                if (heslo != null && heslo.UzivatelskeID == uzivatelID)
                {
                    Databaze.Hesla.Remove(heslo);
                    Databaze.SaveChanges();
                    
                    return RedirectToAction("Zobrazeni");
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        public IActionResult Upravit(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                Heslo? heslo = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                if (heslo != null)
                {
                    klic = Sifrovani.HesloNaKlic(klic);
                    heslo.Sifra = Sifrovani.Desifrovat(klic, heslo.Sifra);
                }

                return View(heslo);
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult Upravit(string sluzba, string jmeno, string heslo, int id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                Heslo? heslo1 = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                if (heslo1 != null)
                {
                    int delka = klic.Length;

                    klic = Sifrovani.HesloNaKlic(klic);

                    int hash = heslo.GetHashCode();
                    heslo = Sifrovani.Zasifrovat(klic, heslo);

                    Heslo h = new Heslo()
                    {
                        UzivatelskeID = (int)uzivatelID,
                        Sluzba = sluzba,
                        Jmeno = jmeno,
                        Hash = hash,
                        Sifra = heslo,
                        ID = id
                    };

                    Databaze.Hesla.Remove(heslo1);
                    Databaze.Hesla.Add(h);
                    Databaze.SaveChanges();

                    return RedirectToAction("Zobrazeni");
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult PotvrditSdileni(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                SdileneHeslo? heslo = Databaze.Sdilena_hesla.Where(heslo => heslo.Id == id).FirstOrDefault();
                if (heslo != null)
                {
                    SdileneHeslo potvrzeno = new()
                    {
                        Id = heslo.Id,
                        PuvodniHesloID = heslo.PuvodniHesloID,
                        ZakladatelID = heslo.ZakladatelID,
                        ZakladatelJmeno = heslo.ZakladatelJmeno,
                        UzivatelskeID = heslo.UzivatelskeID,
                        Potvrzeno = true,
                        Sifra = heslo.Sifra,
                        Sluzba = heslo.Sluzba,
                        Jmeno = heslo.Jmeno
                    };

                    Databaze.Sdilena_hesla.Remove(heslo);
                    Databaze.Sdilena_hesla.Add(potvrzeno);
                    Databaze.SaveChanges();

                    return Ok(Json("ok"));
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult ZrusitSdileni(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                SdileneHeslo? heslo = Databaze.Sdilena_hesla.Where(heslo => heslo.Id == id).FirstOrDefault();
                if (heslo != null)
                {
                    Databaze.Sdilena_hesla.Remove(heslo);
                    Databaze.SaveChanges();

                    return Ok(Json("ok"));
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        public IActionResult Sdileni(int id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                ViewData["id"] = id;
                return View();
            }
            return RedirectToAction("Error", "Home", 404);
        }
    
        [HttpPost]
        public IActionResult Sdileni(int id, Uzivatel obj)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                ModelState.Clear();
                Uzivatel u = Databaze.Uzivatele.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault();
                Heslo h = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                Uzivatel u2 = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();

                SdileneHeslo sh = new()
                {
                    PuvodniHesloID = h.ID,
                    ZakladatelID = (int)uzivatelID,
                    ZakladatelJmeno = u2.Jmeno,
                    UzivatelskeID = u.Id,
                    Sluzba = h.Sluzba,
                    Jmeno = h.Jmeno,
                    Sifra = h.Sifra,
                };

                Databaze.Sdilena_hesla.Add(sh);
                Databaze.SaveChanges();

                return RedirectToAction("Zobrazeni");
            }
            return RedirectToAction("Error", "Home", 404);
        }
    }
}
