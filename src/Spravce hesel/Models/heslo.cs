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

        public string? DesifrovaneHeslo { get; set; } = null;
        public string? DesifrovanaSluzba { get; set; } = null;
        public string? DesifrovaneJmeno { get; set; } = null;
        

        public byte[]? Sluzba { get; set; } = null;

        public byte[]? Jmeno { get; set; } = null;
    }
}
