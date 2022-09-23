using System.ComponentModel.DataAnnotations;


namespace Spravce_hesel.Models
{
    public class uzivatel
    {
        [Key]
        public string email { get; set; }

        [Required]
        public string? username { get; set; } = null;

        [Required]
        public string heslo { get; set; }
    }
}
