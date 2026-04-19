using System.ComponentModel.DataAnnotations;

namespace GestionMateriel.Models
{
    public class Rapport
    {
        public int Id { get; set; }

        [Required]
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [Required]
        [Display(Name = "Commentaire / État du matériel")]
        public string Commentaire { get; set; } = string.Empty;

        [Display(Name = "Matériel rendu en bon état ?")]
        public bool EstEnBonEtat { get; set; } = true;

        [Display(Name = "Date du rapport")]
        public DateTime DateRapport { get; set; } = DateTime.Now;

        public bool EstLu { get; set; } = false;

        [Display(Name = "Problème signalé")]
        public string? ProblemeSignale { get; set; }
    }
}