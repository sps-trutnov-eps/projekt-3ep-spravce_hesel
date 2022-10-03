using System.ComponentModel.DataAnnotations;


namespace Spravce_hesel.Models
{
    public class heslo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int userID { get; set; }

        [Required]
        public string Hash { get; set; }

        [Required]
        public string Sifra { get; set; }
    }
}
