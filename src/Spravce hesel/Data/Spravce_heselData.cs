using Microsoft.EntityFrameworkCore;

namespace Spravce_hesel.Data
{
    public class Spravce_heselData : DbContext
    {
        // public DbSet<Models.ErrorViewModel> Errors { get; set; }
        // Zde bude definován každý model
        // Příklad:
        // public DbSet<Models.Uzivatel> Uzivatele { get; set; }
        // Popis:
        // Models.Uzivatel = Soubor ve složce models se samotným modelem
        // Uzivatele = Název tabulky v databázi (Většinou vývá množné číslo názvu souboru)

        public DbSet<Models.uzivatel> uivatel { get; set; }
        public DbSet<Models.heslo> heslo { get; set; }

        public Spravce_heselData (DbContextOptions<Spravce_heselData> options) : base(options) { }
    }
}