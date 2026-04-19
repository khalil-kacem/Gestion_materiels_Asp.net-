using System.ComponentModel.DataAnnotations;

namespace GestionMateriel.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string EnseignantId { get; set; } = string.Empty;

        public ApplicationUser? Enseignant { get; set; }

        [Required]
        [Display(Name = "Date de début")]
        [DataType(DataType.DateTime)]
        public DateTime DateDebut { get; set; }

        [Required]
        [Display(Name = "Date de retour prévue")]
        [DataType(DataType.DateTime)]
        public DateTime DateRetourPrevue { get; set; }

        [Display(Name = "Statut")]
        public string Statut { get; set; } = "En attente"; // En attente, Approuvée, Refusée, Terminée

        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Relation plusieurs équipements
        public ICollection<ReservationEquipement> ReservationEquipements { get; set; } = new List<ReservationEquipement>();

        [Display(Name = "Commentaire")]
        public string? Commentaire { get; set; }
    }
}