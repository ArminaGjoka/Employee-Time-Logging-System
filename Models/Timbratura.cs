using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Timbratura_Testo.Components;

namespace Timbratura_Testo.Models
{
    public class Timbratura
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string? UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public int? Duration { get; set; }
        public DateTime? RoundedEntryTime { get; set; }
        public DateTime? RoundedExitTime { get; set; }
        public  ApplicationUser? User { get; set; } 
    }

}
