using Microsoft.EntityFrameworkCore;

namespace Spravce_hesel.Data
{
    public class Spravce_heselData : DbContext
    {
        public DbSet<Models.Uzivatel> Uzivatele { get; set; }
        public DbSet<Models.Heslo> Hesla { get; set; }
        public DbSet<Models.SdileneHeslo> Sdilena_hesla { get; set; }

        public Spravce_heselData (DbContextOptions<Spravce_heselData> options) : base(options) { }
    }
}