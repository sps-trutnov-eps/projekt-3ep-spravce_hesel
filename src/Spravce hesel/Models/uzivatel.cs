using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Spravce_hesel.Models
{
    public class uzivatel
    {
        [Required]
        public int Id { get; set; }

        [Key]
        public string? Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Heslo { get; set; }
    }
}
