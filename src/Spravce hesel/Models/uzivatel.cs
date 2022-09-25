using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class uzivatel
    {
        [Key]
        public string? Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Heslo { get; set; }
    }
}
