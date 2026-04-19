using System.ComponentModel.DataAnnotations;

namespace GestionMateriel.Models
{
    public class Equipement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom de l'équipement")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La catégorie est obligatoire")]
        [Display(Name = "Catégorie")]
        public int CategorieId { get; set; }

        public Categorie? Categorie { get; set; }

        [Display(Name = "Disponible")]
        public bool EstDisponible { get; set; } = true;

        [Display(Name = "Date d'acquisition")]
        [DataType(DataType.Date)]
        public DateTime DateAcquisition { get; set; } = DateTime.Now;

        [Display(Name = "Date de retour prévue")]
        [DataType(DataType.Date)]
        public DateTime? DateRetourPrevue { get; set; }

        // Pour empêcher la suppression si réservé
        public bool EstReserve { get; set; } = false;
    }
}