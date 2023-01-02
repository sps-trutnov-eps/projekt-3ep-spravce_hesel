using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spravce_hesel.Classes;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.Text;

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

                    byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);

                    hesla.ForEach(heslo =>
                        heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(heslo.Sluzba, klic, uzivatel.IV));

                    hesla.ForEach(heslo =>
                        heslo.DesifrovaneJmeno = Sifrovani.Desifrovat(heslo.Jmeno, klic, uzivatel.IV));

                    List<string[]> upozorneni = new();
                    List<string> idHesel = new();

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

                    foreach (var hsl in hesla)
                    {
                        if (Sifrovani.Desifrovat(hsl.Sifra, klic, uzivatel.IV).Length < 8)
                        {
                            upozorneni.Add(item: new string[] { hsl.DesifrovanaSluzba, null! });
                        }
                    }

                    foreach (string s in idHesel)
                    {
                        string[] jmena = { hesla[int.Parse(s.Substring(0, s.IndexOf(';')))].DesifrovanaSluzba, hesla[int.Parse(s.Substring(s.LastIndexOf(';') + 1))].DesifrovanaSluzba };

                        upozorneni.Add(jmena);
                    }

                    List<SdileneHeslo> sdilenaHeslaNepotvrzena = Databaze.SdilenaHesla
                        .Where(heslo => heslo.UzivatelskeId == uzivatelId && heslo.Potvrzeno == false).ToList();
                    List<SdileneHeslo> sdilenaHeslaPotvrzena = Databaze.SdilenaHesla
                        .Where(heslo => heslo.UzivatelskeId == uzivatelId && heslo.Potvrzeno == true).ToList();

                    sdilenaHeslaNepotvrzena.ForEach(heslo =>
                        heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(heslo.Sluzba, Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic), uzivatel.IV));

                    sdilenaHeslaPotvrzena.ForEach(heslo =>
                    {
                        if (heslo.Zmeneno && heslo is { DocasnyStringProKlic: { }, Potvrzeno: true })
                        {
                            byte[] docasnyKlic = Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic);
                            string desifrovaneHeslo = Sifrovani.Desifrovat(heslo.Sifra, docasnyKlic, uzivatel.IV);

                            heslo.Zmeneno = false;
                            heslo.Sifra = Sifrovani.Zasifrovat(desifrovaneHeslo, klic, uzivatel.IV);
                            heslo.Jmeno = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Jmeno, docasnyKlic, uzivatel.IV), klic, uzivatel.IV);
                            heslo.Sluzba = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Sluzba, docasnyKlic, uzivatel.IV), klic, uzivatel.IV);

                            Databaze.Entry(heslo).State = EntityState.Modified;
                            Databaze.SaveChanges();
                        }

                        heslo.DesifrovaneHeslo = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                        heslo.DesifrovaneJmeno = Sifrovani.Desifrovat(heslo.Jmeno, klic, uzivatel.IV);
                        heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(heslo.Sluzba, klic, uzivatel.IV);

                        heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(heslo.Sluzba, Sifrovani.HesloNaKlic(hesloUzivatele), uzivatel.IV);
                        heslo.DesifrovaneJmeno = Sifrovani.Desifrovat(heslo.Jmeno, Sifrovani.HesloNaKlic(hesloUzivatele), uzivatel.IV);
                    });

                    ViewData["Oznameni"] = sdilenaHeslaNepotvrzena;
                    ViewData["sdileneHesla"] = sdilenaHeslaPotvrzena;
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
                    heslo.DesifrovaneHeslo = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                    heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(heslo.Sluzba, klic, uzivatel.IV);
                    heslo.DesifrovaneJmeno = Sifrovani.Desifrovat(heslo.Jmeno, klic, uzivatel.IV);

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
                    if (heslo.Zmeneno && heslo is { DocasnyStringProKlic: { }, Potvrzeno: true })
                    {
                        byte[] docasnyKlic = Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic);
                        string desifrovaneHeslo = Sifrovani.Desifrovat(heslo.Sifra, docasnyKlic, uzivatel.IV);

                        heslo.Zmeneno = false;
                        heslo.Sifra = Sifrovani.Zasifrovat(desifrovaneHeslo, klic, uzivatel.IV);
                        heslo.Jmeno = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Jmeno, docasnyKlic, uzivatel.IV), klic, uzivatel.IV);
                        heslo.Sluzba = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Sluzba, docasnyKlic, uzivatel.IV), klic, uzivatel.IV);

                        Databaze.Entry(heslo).State = EntityState.Modified;
                        Databaze.SaveChanges();
                        
                        heslo.DesifrovaneHeslo = desifrovaneHeslo;

                        return Ok(Json(heslo));
                    }

                    heslo.DesifrovaneHeslo = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                    heslo.DesifrovaneJmeno = Sifrovani.Desifrovat(heslo.Jmeno, klic, uzivatel.IV);
                    heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(heslo.Sluzba, klic, uzivatel.IV);

                    return Ok(Json(heslo));
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

                        Heslo h = new()
                        {
                            UzivatelskeId = (int)uzivatelId,
                            Sluzba = Sifrovani.Zasifrovat(sluzba, klic, uzivatel.IV),
                            Jmeno = Sifrovani.Zasifrovat(jmeno, klic, uzivatel.IV),
                            Sifra = Sifrovani.Zasifrovat(heslo, klic, uzivatel.IV)
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
                        heslo.DesifrovaneHeslo = Sifrovani.Desifrovat(heslo.Sifra, klic, uzivatel.IV);
                        heslo.DesifrovaneJmeno = Sifrovani.Desifrovat(heslo.Jmeno, klic, uzivatel.IV);
                        heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(heslo.Sluzba, klic, uzivatel.IV);

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
                                    if (hsl.Potvrzeno)
                                        hsl.Zmeneno = true;
                                    byte[] klic = Sifrovani.HesloNaKlic(docasnyString);
                                    
                                    hsl.Sluzba = Sifrovani.Zasifrovat(sluzba, klic, iv);
                                    hsl.Jmeno = Sifrovani.Zasifrovat(jmeno, klic, iv);
                                    hsl.Sifra = Sifrovani.Zasifrovat(heslo, klic, iv);
                                    hsl.DocasnyStringProKlic = docasnyString;
                                }
                            });

                        byte[] klic = Sifrovani.HesloNaKlic(hesloUzivatele);

                        heslo1.Sluzba = Sifrovani.Zasifrovat(sluzba, klic, uzivatel.IV);
                        heslo1.Jmeno = Sifrovani.Zasifrovat(jmeno, klic, uzivatel.IV);
                        heslo1.Sifra = Sifrovani.Zasifrovat(heslo, klic, uzivatel.IV);

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
                if (uzivatel != null)
                {
                    SdileneHeslo? heslo = Databaze.SdilenaHesla.FirstOrDefault(heslo => heslo.Id == id);
                    if (heslo is { DocasnyStringProKlic: { }, Potvrzeno: false })
                    {
                        heslo.Potvrzeno = true;
                        heslo.Sifra = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Sifra, Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic),
                            uzivatel.IV), Sifrovani.HesloNaKlic(hesloUzivatele), uzivatel.IV);
                        heslo.Jmeno = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Jmeno, Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic),
                            uzivatel.IV), Sifrovani.HesloNaKlic(hesloUzivatele), uzivatel.IV);
                        heslo.Sluzba = Sifrovani.Zasifrovat(Sifrovani.Desifrovat(heslo.Sluzba, Sifrovani.HesloNaKlic(heslo.DocasnyStringProKlic),
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
                if (heslo != null && heslo.Potvrzeno == false)
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

                Uzivatel uzivatel = Databaze.Uzivatele.FirstOrDefault(uzivatel => uzivatel.Id == uzivatelId);

                List<SdileneHeslo> sdilenaHesla = Databaze.SdilenaHesla.Where(heslo => heslo.PuvodniHesloId == id).ToList();

                sdilenaHesla.ForEach(heslo =>
                {
                    if (heslo.ZakladatelId == uzivatelId)
                    {
                        Heslo? puvodniHeslo = Databaze.Hesla.FirstOrDefault(pHeslo => pHeslo.Id == heslo.PuvodniHesloId);
                        if (puvodniHeslo != null)
                        {
                            heslo.DesifrovanaSluzba = Sifrovani.Desifrovat(puvodniHeslo.Sluzba, Sifrovani.HesloNaKlic(klic), uzivatel.IV);
                            heslo.DesifrovaneJmeno = Sifrovani.Desifrovat(puvodniHeslo.Jmeno, Sifrovani.HesloNaKlic(klic), uzivatel.IV);
                        }
                    }
                });
                
                ViewData["id"] = id;
                ViewData["sdileneHesla"] = sdilenaHesla;
                
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
                    
                    string stringDocasnehoKlice = Sifrovani.InfoProKlic(12);
                    byte[] klic2 = Sifrovani.HesloNaKlic(stringDocasnehoKlice);

                    string desifrovaneHeslo = Sifrovani.Desifrovat(h.Sifra, klic, u2.IV);
                    string desifrovanaSluzba = Sifrovani.Desifrovat(h.Sluzba, klic, u2.IV);
                    string desifrovaneJmeno = Sifrovani.Desifrovat(h.Jmeno, klic, u2.IV);

                    if (ModelState.IsValid)
                    {
                        SdileneHeslo sh = new()
                        {
                            PuvodniHesloId = h.Id,
                            ZakladatelId = (int)uzivatelId,
                            ZakladatelJmeno = u2.Jmeno + " (" + u2.Email + ")",
                            UzivatelskeId = u.Id,
                            UzivatelskeJmeno = u.Jmeno + " (" + u.Email + ")",
                            Sluzba = Sifrovani.Zasifrovat(desifrovanaSluzba, klic2, u.IV),
                            Jmeno = Sifrovani.Zasifrovat(desifrovaneJmeno, klic2, u.IV),
                            Sifra = Sifrovani.Zasifrovat(desifrovaneHeslo, klic2, u.IV),
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
