using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.ComponentModel.DataAnnotations;
using Spravce_hesel.Classes;
using System.Diagnostics;
using Microsoft.AspNetCore.Connections.Features;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && heslo_uzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null)
                {
                    byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);
                    List<Heslo> Hesla = Databaze.Hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID).ToList();
                    List<Heslo> desifrovano = new();

                    foreach (Heslo heslo in Hesla)
                    {
                        Heslo _desforave = new()
                        {
                            ID = heslo.ID,
                            UzivatelskeID = heslo.UzivatelskeID,
                            Sluzba = heslo.Sluzba,
                            Jmeno = heslo.Jmeno,
                            Hash = heslo.Hash,
                            desifrovano = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV)
                        };

                        desifrovano.Add(_desforave);
                    }

                    ViewData["Oznameni"] = Databaze.Sdilena_hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID)
                        .Where(heslo => heslo.Potvrzeno == false).ToList();
                    ViewData["sdilene_hesla"] = Databaze.Sdilena_hesla.Where(heslo => heslo.UzivatelskeID == uzivatelID)
                        .Where(heslo => heslo.Potvrzeno == true).ToList();

                    return View(desifrovano);
                }
            }
            return StatusCode(401);
        }

        [HttpGet] 
        public IActionResult DetailHesla(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            Heslo? heslo = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
            if (uzivatelID != null && heslo_uzivatele != null && heslo != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null
                    && heslo.UzivatelskeID == uzivatelID)
                {
                    byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);
                    heslo.desifrovano = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                    return Ok(Json(heslo));
                }
            }

            return StatusCode(401);
        }

        [HttpGet]
        public IActionResult DetailSdilenehoHesla(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            SdileneHeslo? heslo = Databaze.Sdilena_hesla.Where(heslo => heslo.Id == id).FirstOrDefault();
            if (uzivatelID != null && heslo_uzivatele != null && heslo != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null
                    && heslo.UzivatelskeID == uzivatelID)
                {
                    byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);
                    heslo.desifrovano = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                    return Ok(Json(heslo));
                }
            }

            return StatusCode(401);
        }

        [HttpGet]
        public IActionResult Pridat()
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            if (uzivatelID != null && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                return View();
            }

            return StatusCode(401);
        }

        [HttpPost]
        public IActionResult Pridat(string sluzba, string jmeno, string heslo)
        {
            ModelState.Clear();

            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && heslo_uzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null)
                {
                    if (heslo == null || heslo.Length == 0)
                    {
                        ModelState.AddModelError("Jmeno", "Vyplňte toto pole!");
                    }
                    else if (ModelState.IsValid)
                    {
                        int delka = heslo_uzivatele.Length;

                        byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);

                        int hash = heslo.GetHashCode();
                        byte[] sifra = Sifrovani.Zasifrovat(heslo, klic, uzivatel.IV);

                        Heslo h = new()
                        {
                            UzivatelskeID = (int)uzivatelID,
                            Sluzba = sluzba,
                            Jmeno = jmeno,
                            Hash = hash,
                            Sifra = sifra
                        };

                        Databaze.Hesla.Add(h);
                        Databaze.SaveChanges();

                        return RedirectToAction("Zobrazeni");
                    }

                    return View();
                }
            }

            return StatusCode(500);
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
                    List<SdileneHeslo> Sdileni = Databaze.Sdilena_hesla.Where(heslo => heslo.PuvodniHesloID == id).ToList();

                    Databaze.Hesla.Remove(heslo);
                    Databaze.RemoveRange(Sdileni);

                    Databaze.SaveChanges();
                    
                    return RedirectToAction("Zobrazeni");
                }
            }

            return StatusCode(500);
        }

        [HttpPost]
        public IActionResult OdstranitSdilene(int id = 0)
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

                    return RedirectToAction("Zobrazeni");
                }
            }

            return StatusCode(500);
        }

        [HttpGet]
        public IActionResult Upravit(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && heslo_uzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null)
                {
                    Heslo? heslo = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                    if (heslo != null)
                    {
                        byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);
                        heslo.desifrovano = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                    }

                    return View(heslo);
                }
            }

            return StatusCode(401);
        }

        [HttpPost]
        public IActionResult Upravit(string sluzba, string jmeno, string heslo, int id)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && heslo_uzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (uzivatel != null)
                {
                    Heslo? heslo1 = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                    if (heslo1 != null)
                    {
                        int delka = heslo_uzivatele.Length;

                        byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);

                        int hash = heslo.GetHashCode();
                        byte[] sifra = Sifrovani.Zasifrovat(heslo, klic, uzivatel.IV);

                        Heslo h = new()
                        {
                            UzivatelskeID = (int)uzivatelID,
                            Sluzba = sluzba,
                            Jmeno = jmeno,
                            Hash = hash,
                            Sifra = sifra,
                            ID = id
                        };

                        Databaze.Hesla.Remove(heslo1);
                        Databaze.Hesla.Add(h);
                        Databaze.SaveChanges();

                        return RedirectToAction("Zobrazeni");
                    }
                }
            }

            return StatusCode(500);
        }

        [HttpPost]
        public IActionResult PotvrditSdileni(int id = 0)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && heslo_uzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if(uzivatel != null)
                {
                    SdileneHeslo? heslo = Databaze.Sdilena_hesla.Where(heslo => heslo.Id == id).FirstOrDefault();
                    if (heslo != null && heslo.DocasnyStringProKlic != null)
                    {
                        byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);
                        byte[] klic2 = Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic);
                        string desifrovane = Sifrovani.Desifrovat(heslo.Sifra, klic2, uzivatel.IV);
                        SdileneHeslo potvrzeno = new()
                        {
                            Id = heslo.Id,
                            PuvodniHesloID = heslo.PuvodniHesloID,
                            ZakladatelID = heslo.ZakladatelID,
                            ZakladatelJmeno = heslo.ZakladatelJmeno,
                            UzivatelskeID = heslo.UzivatelskeID,
                            Potvrzeno = true,
                            Sifra = Sifrovani.Zasifrovat(desifrovane, klic, uzivatel.IV),
                            Sluzba = heslo.Sluzba,
                            Jmeno = heslo.Jmeno
                        };

                        Databaze.Sdilena_hesla.Remove(heslo);
                        Databaze.Sdilena_hesla.Add(potvrzeno);
                        Databaze.SaveChanges();

                        return Ok(Json("ok"));
                    }
                }
            }

            return StatusCode(500);
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

            return StatusCode(500);
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
            return StatusCode(401);
        }
    
        [HttpPost]
        public IActionResult Sdileni(int? id, Uzivatel? obj)
        {
            int? uzivatelID = HttpContext.Session.GetInt32("ID");
            string? heslo_uzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelID != null && heslo_uzivatele != null && obj != null
                && Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() != null)
            {
                ModelState.Clear();

                Uzivatel? u = Databaze.Uzivatele.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault();
                Heslo? h = Databaze.Hesla.Where(heslo => heslo.ID == id).FirstOrDefault();
                Uzivatel? u2 = Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault();
                if (u == null)
                {
                    ModelState.AddModelError("Email", "Tento uživatel neexistuje.");
                    return View();
                }
                if (u != null && h != null && u2 != null)
                {
                    SdileneHeslo? existujiciHeslo = Databaze.Sdilena_hesla
                        .Where(heslo => heslo.PuvodniHesloID == h.ID)
                        .Where(heslo => heslo.UzivatelskeID == u.Id).FirstOrDefault();

                    if (existujiciHeslo != null)
                    {
                        ModelState.AddModelError("Email", "Toto heslo už sdílíte.");
                    }

                    if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Id == uzivatelID).FirstOrDefault() == Databaze.Uzivatele.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault())
                    {
                        ModelState.AddModelError("Email", "Nemůžete sdílet heslo sami se sebou.");
                    }

                    if (Databaze.Uzivatele.Where(uzivatel => uzivatel.Email == obj.Email).FirstOrDefault() == null)
                    {
                        ModelState.AddModelError("Email", "Uživatel neexistuje.");
                    }
                    byte[] klic = Sifrovani.HesloNaKlic(heslo_uzivatele);
                    string desifrovano = Sifrovani.Desifrovat(h.Sifra, klic, u2.IV);
                    string string_docasneho_klice = Sifrovani.Nahodne_info_pro_klic(12);
                    byte[] klic2 = Sifrovani.HesloNaKlic(string_docasneho_klice);
                    
                    if (ModelState.IsValid)
                    {
                        SdileneHeslo sh = new()
                        {
                            PuvodniHesloID = h.ID,
                            ZakladatelID = (int)uzivatelID,
                            ZakladatelJmeno = u2.Jmeno + " (" + u2.Email + ")",
                            UzivatelskeID = u.Id,
                            Sluzba = h.Sluzba,
                            Jmeno = h.Jmeno,
                            Sifra = Sifrovani.Zasifrovat(desifrovano, klic2, u.IV),
                            DocasnyStringProKlic = string_docasneho_klice
                        };
                
                        Databaze.Sdilena_hesla.Add(sh);
                        Databaze.SaveChanges();

                        return RedirectToAction("Zobrazeni");
                    }

                    return View();
                }
            }

            return StatusCode(500);
        }
    }
}
