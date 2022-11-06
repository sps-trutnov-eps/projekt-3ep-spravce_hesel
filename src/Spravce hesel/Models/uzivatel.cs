using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class Uzivatel
    {
        [Key]
        public string Email { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public string Jmeno { get; set; }

        [Required]
        public string Heslo { get; set; }

        [Required]
        public byte[] IV { get; set; } //slouzi k sifrovani
    }
}
