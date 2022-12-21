using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class SdileneHeslo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PuvodniHesloId { get; set; }

        [Required]
        public int ZakladatelId { get; set; }

        [Required]
        public string ZakladatelJmeno { get; set; }

        [Required]
        public int UzivatelskeId { get; set; }

        [Required]
        public string UzivatelskeJmeno { get; set; }

        [Required]
        public bool Potvrzeno { get; set; } = false;

        [Required]
        public bool Zmeneno { get; set; } = false;

        [Required]
        public byte[] Sifra { get; set; }
        
        public string? DesifrovaneHeslo { get; set; }
        public string? DesifrovanaSluzba { get; set; }
        public string? DesifrovaneJmeno { get; set; }
        
        public byte[] Sluzba { get; set; }

        public byte[] Jmeno { get; set; }

        public string? DocasnyStringProKlic { get; set; } = null;
    }
}
