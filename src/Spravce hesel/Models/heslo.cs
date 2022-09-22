using System.ComponentModel.DataAnnotations;


namespace Spravce_hesel.Models
{
    public class heslo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string hash { get; set; }

        [Required]
        public string sifra { get; set; }
    }
}
