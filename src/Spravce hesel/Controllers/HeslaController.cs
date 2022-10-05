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
            string? klic = HttpContext.Session.GetString("Heslo");
            int? IDuzivatele = HttpContext.Session.GetInt32("ID");
            if (IDuzivatele == null || klic == null)
            {
                return RedirectToAction("Home", "Index");
            }


            int hash = heslo.GetHashCode();
            heslo = Sifrovani.Zasifrovat(klic, heslo);

            Heslo h = new Heslo();
            h.UzivatelskeID = (int)IDuzivatele;
            h.Sluzba = sluzba;
            h.Jmeno = jmeno;
            h.Hash = hash;
            h.Sifra = heslo;
            Databaze.Hesla.Add(h);
            return RedirectToAction("Zobrazeni");
        }
    }
}
