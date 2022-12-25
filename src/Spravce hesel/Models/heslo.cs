using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class Heslo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UzivatelskeId { get; set; }

        [Required]
        public byte[] Sifra { get; set; }

        public string DesifrovaneHeslo { get; set; } = String.Empty;
        public string DesifrovanaSluzba { get; set; } = String.Empty;
        public string DesifrovaneJmeno { get; set; } = String.Empty;

        public byte[] Sluzba { get; set; }

        public byte[] Jmeno { get; set; }
    }
}
