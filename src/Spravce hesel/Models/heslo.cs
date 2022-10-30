using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class Heslo
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int UzivatelskeID { get; set; }

        [Required]
        public int Hash { get; set; }

        [Required]
        public string Sifra { get; set; }

        public string? Sluzba { get; set; } = null;

        public string? Jmeno { get; set; } = null;
    }
}
