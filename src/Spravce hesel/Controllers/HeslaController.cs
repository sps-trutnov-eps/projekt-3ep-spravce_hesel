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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace Spravce_hesel.Controllers
{
    
    public class HeslaController : Controller
    {
        private SpravceHeselData Databaze { get; set; }
        public HeslaController(SpravceHeselData databaze)
        {
            Databaze = databaze;
        }

        [HttpGet]
        public IActionResult Zobrazeni()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && hesloUzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if (uzivatel != null)
                {
                    List<Heslo> hesla = Databaze.Hesla.Where(heslo => heslo.UzivatelskeId == uzivatelId).ToList();

                    List<string[]> upozorneni = new List<string[]>();
                    List<string> idHesel = new List<string>();
                    byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);

                    for (int i = 0; i < hesla.Count; i++)
                    {
                        for (int l = 0; l < hesla.Count; l++)
                        {
                            if (Sifrovani.Desifrovat(hesla[i].Sifra, klic, uzivatel.IV) == Sifrovani.Desifrovat(hesla[l].Sifra, klic, uzivatel.IV) && i != l)
                            {
                                if (i < l)
                                {
                                    idHesel.Add(i.ToString() + ";" + l.ToString());
                                }
                                else
                                {
                                    idHesel.Add(l.ToString() + ";" + i.ToString());
                                }
                            }
                        }
                    }

                    idHesel = idHesel.Distinct().ToList();

                    foreach (string s in idHesel)
                    {
                        string[] jmena = { hesla[int.Parse(s.Substring(0, s.IndexOf(';')))].Sluzba, hesla[int.Parse(s.Substring(s.LastIndexOf(';') + 1))].Sluzba };

                        upozorneni.Add(jmena);
                    }

                    for (int i = 0; i < hesla.Count; i++)
                    {
                        if (Sifrovani.Desifrovat(hesla[i].Sifra, klic, uzivatel.IV).Length < 8)
                        {
                            upozorneni.Add(new string[] {hesla[i].Sluzba, null});
                        }
                    }
                    ViewData["Oznameni"] = Databaze.SdilenaHesla.Where(heslo => heslo.UzivatelskeId == uzivatelId)
                        .Where(heslo => heslo.Potvrzeno == false).ToList();
                    ViewData["sdilene_hesla"] = Databaze.SdilenaHesla.Where(heslo => heslo.UzivatelskeId == uzivatelId)
                        .Where(heslo => heslo.Potvrzeno == true).ToList();
                    ViewData["upozorneni"] = upozorneni;

                    return View(hesla);
                }
            }

            return StatusCode(401);
        }

        [HttpGet] 
        public IActionResult DetailHesla(int id = 0)
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            Heslo? heslo = Databaze.Hesla.FirstOrDefault(heslo => heslo.Id == id);
            if (uzivatelId != null && hesloUzivatele != null && heslo != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if (uzivatel != null
                    && heslo.UzivatelskeId == uzivatelId)
                {
                    byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);
                    heslo.Desifrovano = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                    return Ok(Json(heslo));
                }
            }

            return StatusCode(401);
        }

        [HttpGet]
        public IActionResult DetailSdilenehoHesla(int id = 0)
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            SdileneHeslo? heslo = Databaze.SdilenaHesla.FirstOrDefault(heslo => heslo.Id == id);
            if (uzivatelId != null && hesloUzivatele != null && heslo != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if (uzivatel != null
                    && heslo.UzivatelskeId == uzivatelId)
                {
                    byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);
                    if (heslo.Zmeneno == true && heslo.DocasnyStringProKlic != null && heslo.Potvrzeno)
                    {
                        Databaze.SdilenaHesla.Remove(heslo);
                        byte[] docasny_klic = Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic);
                        string Desifrovano = Sifrovani.Desifrovat(heslo.Sifra, docasny_klic, uzivatel.IV);
                        SdileneHeslo upravene = new SdileneHeslo
                        {

                            PuvodniHesloId = heslo.PuvodniHesloId,
                            ZakladatelId = heslo.ZakladatelId,
                            ZakladatelJmeno = heslo.ZakladatelJmeno,
                            UzivatelskeId = heslo.UzivatelskeId,
                            UzivatelskeJmeno = heslo.UzivatelskeJmeno,
                            Potvrzeno = heslo.Potvrzeno,
                            Zmeneno = false,
                            Sifra = Sifrovani.Zasifrovat(Desifrovano, klic, uzivatel.IV),
                            Sluzba = heslo.Sluzba,
                            Jmeno = heslo.Jmeno,
                        };
                        Databaze.SdilenaHesla.Add(upravene);
                        Databaze.SaveChanges();
                        upravene.Desifrovano = Desifrovano;
                        return Ok(Json(upravene));
                    }
                    else
                    {
                        heslo.Desifrovano = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                        return Ok(Json(heslo));
                    }

                    
                }
            }

            return StatusCode(401);
        }

        [HttpGet]
        public IActionResult Pridat()
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            if (uzivatelId != null && Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
            {
                return View();
            }

            return StatusCode(401);
        }

        [HttpPost]
        public IActionResult Pridat(string sluzba, string jmeno, string heslo)
        {
            ModelState.Clear();

            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && hesloUzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if (uzivatel != null)
                {
                    if (heslo == null || heslo.Length == 0)
                    {
                        ModelState.AddModelError("Jmeno", "Vyplňte toto pole!");
                    }
                    else if (ModelState.IsValid)
                    {
                        byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);
                        
                        byte[] sifra = Sifrovani.Zasifrovat(heslo, klic, uzivatel.IV);

                        Heslo h = new()
                        {
                            UzivatelskeId = (int)uzivatelId,
                            Sluzba = sluzba,
                            Jmeno = jmeno,
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
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && klic != null
                && Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
            {
                Heslo? heslo = Databaze.Hesla.FirstOrDefault(heslo => heslo.UzivatelskeId == uzivatelId && heslo.Id == id);
                if (heslo != null && heslo.UzivatelskeId == uzivatelId)
                {
                    List<SdileneHeslo> sdileni = Databaze.SdilenaHesla.Where(hesl => hesl.PuvodniHesloId == id).ToList();

                    Databaze.Hesla.Remove(heslo);
                    Databaze.RemoveRange(sdileni);

                    Databaze.SaveChanges();
                    
                    return RedirectToAction("Zobrazeni");
                }
            }

            return StatusCode(500);
        }

        [HttpPost]
        public IActionResult OdstranitSdilene(int id = 0)
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && klic != null
                && Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
            {
                SdileneHeslo? heslo = Databaze.SdilenaHesla.FirstOrDefault(heslo => heslo.Id == id);
                if (heslo != null)
                {
                    Databaze.SdilenaHesla.Remove(heslo);
                    Databaze.SaveChanges();

                    return RedirectToAction("Zobrazeni");
                }
            }

            return StatusCode(500);
        }

        [HttpGet]
        public IActionResult Upravit(int id = 1)
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && hesloUzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if (uzivatel != null)
                {
                    Heslo? heslo = Databaze.Hesla.FirstOrDefault(heslo => heslo.Id == id);
                    if (heslo != null)
                    {
                        byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);
                        heslo.Desifrovano = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);

                        string sdilenoPro = "";
                        Databaze.SdilenaHesla.Where(hesl => hesl.PuvodniHesloId == id).ToList().ForEach(hesl => sdilenoPro += hesl.UzivatelskeJmeno + ", ");
                        if (sdilenoPro.Length > 2)
                            ViewData["sdilenoPro"] = sdilenoPro.Remove(sdilenoPro.Length - 2);

                        return View(heslo);
                    }
                }
            }

            return StatusCode(401);
        }

        [HttpPost]
        public IActionResult Upravit(string sluzba, string jmeno, string heslo, int id)
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && hesloUzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if (uzivatel != null)
                {
                    Heslo? heslo1 = Databaze.Hesla.FirstOrDefault(hesl => hesl.Id == id);
                    if (heslo1 != null)
                    {
                        List<SdileneHeslo> sdileneHesla = Databaze.SdilenaHesla.Where(heslo => heslo.PuvodniHesloId == id).ToList();
                        sdileneHesla.ForEach(hsl =>
                            {
                                byte[]? iv = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == hsl.UzivatelskeId).IV;
                                if (iv != null)
                                {
                                    string docasnyString = Sifrovani.InfoProKlic(8);
                                    hsl.Zmeneno = true;
                                    hsl.Sluzba = sluzba;
                                    hsl.Jmeno = jmeno;
                                    hsl.Sifra = Sifrovani.Zasifrovat(heslo, Sifrovani.HesloNaKlic(docasnyString), iv);
                                    hsl.DocasnyStringProKlic = docasnyString;
                                }
                            });

                        heslo1.Sluzba = sluzba;
                        heslo1.Jmeno = jmeno;
                        heslo1.Sifra = Sifrovani.Zasifrovat(heslo, Sifrovani.HesloNaKlic(hesloUzivatele), uzivatel.IV);

                        Databaze.Entry(heslo1).State = EntityState.Modified;
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
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && hesloUzivatele != null)
            {
                Uzivatel? uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if(uzivatel != null)
                {
                    SdileneHeslo? heslo = Databaze.SdilenaHesla.FirstOrDefault(heslo => heslo.Id == id);
                    if (heslo is { DocasnyStringProKlic: { }, Potvrzeno: false })
                    {
                        heslo.Potvrzeno = true;
                        heslo.Sifra = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Sifra, Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic),
                            uzivatel.IV), Sifrovani.HesloNaKlic(hesloUzivatele), uzivatel.IV);

                        Databaze.Entry(heslo).State = EntityState.Modified;
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
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && klic != null
                && Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
            {
                SdileneHeslo? heslo = Databaze.SdilenaHesla.FirstOrDefault(heslo => heslo.Id == id);
                if (heslo != null)
                {
                    Databaze.SdilenaHesla.Remove(heslo);
                    Databaze.SaveChanges();

                    return Ok(Json("ok"));
                }
            }

            return StatusCode(500);
        }

        [HttpGet]
        public IActionResult Sdileni(int id)
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? klic = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && klic != null
                && Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
            {
                ViewData["id"] = id;
                ViewData["sdileneHesla"] = Databaze.SdilenaHesla.Where(heslo => heslo.PuvodniHesloId == id).ToList();
                return View();
            }
            return StatusCode(401);
        }
    
        [HttpPost]
        public IActionResult Sdileni(int? id, Uzivatel? obj)
        {
            int? uzivatelId = HttpContext.Session.GetInt32("ID");
            string? hesloUzivatele = HttpContext.Session.GetString("Klic");
            if (uzivatelId != null && hesloUzivatele != null && obj != null
                && Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) != null)
            {
                ModelState.Clear();

                Uzivatel? u = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Email == obj.Email);
                Heslo? h = Databaze.Hesla.FirstOrDefault(heslo => heslo.Id == id);
                Uzivatel? u2 = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);
                if (u == null)
                {
                    ModelState.AddModelError("Email", "Tento uživatel neexistuje.");
                    return RedirectToAction("Sdileni", id);
                }
                if (h != null && u2 != null)
                {
                    SdileneHeslo? existujiciHeslo = Databaze.SdilenaHesla
                        .FirstOrDefault(heslo => heslo.PuvodniHesloId == h.Id && heslo.UzivatelskeId == u.Id);

                    if (existujiciHeslo != null)
                    {
                        ModelState.AddModelError("Email", "Toto heslo už sdílíte.");
                    }

                    if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId) == Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Email == obj.Email))
                    {
                        ModelState.AddModelError("Email", "Nemůžete sdílet heslo sami se sebou.");
                    }

                    if (Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Email == obj.Email) == null)
                    {
                        ModelState.AddModelError("Email", "Uživatel neexistuje.");
                    }
                    byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);
                    string desifrovano = Sifrovani.Desifrovat(h.Sifra, klic, u2.IV);
                    string stringDocasnehoKlice = Sifrovani.InfoProKlic(12);
                    byte[] klic2 = Sifrovani.HesloNaKlic(stringDocasnehoKlice);
                    
                    if (ModelState.IsValid)
                    {
                        SdileneHeslo sh = new()
                        {
                            PuvodniHesloId = h.Id,
                            ZakladatelId = (int)uzivatelId,
                            ZakladatelJmeno = u2.Jmeno + " (" + u2.Email + ")",
                            UzivatelskeId = u.Id,
                            UzivatelskeJmeno = u.Jmeno + " (" + u.Email + ")",
                            Sluzba = h.Sluzba,
                            Jmeno = h.Jmeno,
                            Sifra = Sifrovani.Zasifrovat(desifrovano, klic2, u.IV),
                            DocasnyStringProKlic = stringDocasnehoKlice
                        };
                
                        Databaze.SdilenaHesla.Add(sh);
                        Databaze.SaveChanges();

                        return RedirectToAction("Zobrazeni");
                    }

                    return RedirectToAction("Sdileni", id);
                }
            }

            return StatusCode(500);
        }
    }
}
