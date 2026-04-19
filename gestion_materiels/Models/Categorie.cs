using System.ComponentModel.DataAnnotations;

namespace GestionMateriel.Models
{
    public class Categorie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la catégorie est obligatoire")]
        [Display(Name = "Nom de la catégorie")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}