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
            if (uzivatelID != null)
            {
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    List<Heslo> Hesla = Databaze.Hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).ToList();

                    return View(Hesla);
                }
            }

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet] 
        public IActionResult DetailHesla(int id)
        {
            Heslo? heslo = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();

            return Ok(Json(heslo));
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
                if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
                {
                    int delka = klic.Length;

                    klic = "b14ca5898a4e4133bbce2ea2315a1916"; // docasny klic, pak musim vymyslet jak ho vyrobit

                    int hash = heslo.GetHashCode();
                    heslo = Sifrovani.Zasifrovat(klic, heslo);

                    Heslo h = new Heslo()
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
            }

            return RedirectToAction("Error", "Home", 404);
        }
    }
}
