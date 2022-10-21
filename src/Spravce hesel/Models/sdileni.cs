using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class SdileneHeslo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ZakladatelID { get; set; }

        [Required]
        public int UzivatelskeID { get; set; }

        [Required]
        public string Sifra { get; set; }

        [Required]
        public bool Potvrzeno { get; set; } = false;
    }
}
