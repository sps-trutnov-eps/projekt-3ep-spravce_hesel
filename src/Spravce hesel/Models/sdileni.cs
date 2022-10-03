using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class SdileneHeslo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdHesla { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string SdilenaSifra { get; set; }
    }
}
