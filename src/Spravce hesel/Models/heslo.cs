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

        public string? Desifrovano { get; set; } = null;

        public string? Sluzba { get; set; } = null;

        public string? Jmeno { get; set; } = null;
    }
}
