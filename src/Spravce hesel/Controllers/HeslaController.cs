using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.ComponentModel.DataAnnotations;

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
            return RedirectToAction("Zobrazeni");
        }
    }
}
