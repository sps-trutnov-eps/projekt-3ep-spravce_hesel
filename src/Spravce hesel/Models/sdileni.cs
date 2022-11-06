using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class SdileneHeslo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PuvodniHesloID { get; set; }

        [Required]
        public int ZakladatelID { get; set; }

        [Required]
        public string ZakladatelJmeno { get; set; }

        [Required]
        public int UzivatelskeID { get; set; }

        [Required]
        public bool Potvrzeno { get; set; } = false;

        [Required]
        public bool zmeneno { get; set; } = false;

        [Required]
        public byte[] Sifra { get; set; }

        public string? desifrovano { get; set; } = null;

        public string? Sluzba { get; set; } = null;

        public string? Jmeno { get; set; } = null;

        public string? DocasnyStringProKlic { get; set; } = null;
    }
}
