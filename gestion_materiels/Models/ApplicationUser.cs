using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GestionMateriel.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Le nom complet est obligatoire")]
        [Display(Name = "Nom complet")]
        public string NomComplet { get; set; } = string.Empty;

        [Display(Name = "Date d'inscription")]
        public DateTime DateInscription { get; set; } = DateTime.Now;
    }
}