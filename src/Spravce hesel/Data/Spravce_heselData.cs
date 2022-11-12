using Microsoft.EntityFrameworkCore;

namespace Spravce_hesel.Data
{
    public class SpravceHeselData : DbContext
    {
        public DbSet<Models.Uzivatel> Uzivatele { get; set; }
        public DbSet<Models.Heslo> Hesla { get; set; }
        public DbSet<Models.SdileneHeslo> SdilenaHesla { get; set; }

        public SpravceHeselData(DbContextOptions<SpravceHeselData> options) : base(options) { }
    }
}