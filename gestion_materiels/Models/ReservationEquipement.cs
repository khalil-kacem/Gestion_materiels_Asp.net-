namespace GestionMateriel.Models
{
    public class ReservationEquipement
    {
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        public int EquipementId { get; set; }
        public Equipement? Equipement { get; set; }
    }
}