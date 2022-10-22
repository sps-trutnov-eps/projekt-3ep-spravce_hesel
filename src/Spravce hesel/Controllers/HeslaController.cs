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
            if (uzivatelID != null && klic != null)
            {
                klic = Sifrovani.HesloNaKlic(klic);
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
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
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet] 
        public IActionResult DetailHesla(int id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    Heslo? heslo = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                    if (heslo != null)
                    {
                        if (heslo.UzivatelskeID == uzivatelID)
                        {
                            klic = Sifrovani.HesloNaKlic(klic);
                            heslo.Sifra = Sifrovani.Desifrovat(klic, heslo.Sifra);
                            return Ok(Json(heslo));
                        }
                    }
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        public IActionResult Pridat()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    return View();
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult Pridat(string sluzba, string jmeno, string heslo)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null)
            {
                Uzivatel uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null)
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

                    SdileneHeslo sh = new()
                    {
                        PuvodniHesloID = h.ID,
                        ZakladatelID = (int)uzivatelID,
                        ZakladatelJmeno = uzivatel.Jmeno,
                        UzivatelskeID = (int)uzivatelID,
                        Sluzba = sluzba,
                        Jmeno = jmeno,
                        Sifra = heslo,
                    };

                    Databaze.Hesla.Add(h);
                    Databaze.Sdilena_hesla.Add(sh);
                    Databaze.SaveChanges();

                    return RedirectToAction("Zobrazeni");
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult Odstranit(int? id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && id != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    Heslo heslo = Databaze.Hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).Where(heslo => heslo.ID == id).FirstOrDefault();
                    if (heslo != null)
                    {
                        Databaze.Hesla.Remove(heslo);
                        Databaze.SaveChanges();
                    
                        return RedirectToAction("Zobrazeni");

                    }
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        public IActionResult Upravit(int? id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {

                    Heslo heslo = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                    klic = Sifrovani.HesloNaKlic(klic);
                    heslo.Sifra = Sifrovani.Desifrovat(klic, heslo.Sifra);
                    return View(heslo);
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        public IActionResult Upravit(string sluzba, string jmeno, string heslo, int id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    Heslo? heslo1 = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();

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

        [HttpGet]
        public IActionResult PotvrditSdileni(int id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null)
            {
                SdileneHeslo? heslo = Databaze.Sdilena_hesla.Where(heslo => heslo.Id == id).FirstOrDefault();
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null && heslo != null)
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

                    return RedirectToAction("Zobrazeni");
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        public IActionResult ZrusitSdileni(int id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && klic != null)
            {
                SdileneHeslo? heslo = Databaze.Sdilena_hesla.Where(heslo => heslo.Id == id).FirstOrDefault();
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null && heslo != null)
                {
                    Databaze.Sdilena_hesla.Remove(heslo);
                    Databaze.SaveChanges();

                    return RedirectToAction("Zobrazeni");
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }
    }
}
