using System.ComponentModel.DataAnnotations;

namespace Timbratura_Testo.Models
{
    public class Utenti
    {
        [EmailAddress]
        [Required]
        public string LoginName { get; set; } = string.Empty;

        [Required]
        public string UserRole { get; set; } = string.Empty;


    }
}
