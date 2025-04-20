using Microsoft.AspNetCore.Identity;

namespace Timbratura_Testo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Timbratura>? Timbrature { get; set; }
   
    }
}

