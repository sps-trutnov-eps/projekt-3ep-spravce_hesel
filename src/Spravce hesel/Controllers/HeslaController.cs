using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.ComponentModel.DataAnnotations;
using Spravce_hesel.Classes;

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
            return View();
        }

        [HttpGet]
        public IActionResult Pridat()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Pridat(string sluzba, string jmeno, string heslo)
        {
            string? klic = HttpContext.Session.GetString("Klic");
            int? IDuzivatele = HttpContext.Session.GetInt32("ID");
            if (IDuzivatele == null || klic == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int delka = klic.Length;

            klic = "b14ca5898a4e4133bbce2ea2315a1916"; // docasny klic, pak musim vymislet jak ho vyrobit


            int hash = heslo.GetHashCode();
            heslo = Sifrovani.Zasifrovat(klic, heslo);

            Heslo h = new Heslo();
            h.UzivatelskeID = (int)IDuzivatele;
            h.Sluzba = sluzba;
            h.Jmeno = jmeno;
            h.Hash = hash;
            h.Sifra = heslo;
            Databaze.Hesla.Add(h);
            Databaze.SaveChanges();
            return RedirectToAction("Zobrazeni");
        }
    }
}
