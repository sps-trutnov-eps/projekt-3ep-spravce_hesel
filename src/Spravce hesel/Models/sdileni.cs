using System.ComponentModel.DataAnnotations;

namespace Spravce_hesel.Models
{
    public class sdileni
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Id_hesla { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string sdilena_sifra { get; set; }

    }
}
