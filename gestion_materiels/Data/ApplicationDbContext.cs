using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionMateriel.Models;

namespace GestionMateriel.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Equipement> Equipements { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Rapport> Rapports { get; set; }
        public DbSet<ReservationEquipement> ReservationEquipements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuration de la relation plusieurs-à-plusieurs
            builder.Entity<ReservationEquipement>()
                .HasKey(re => new { re.ReservationId, re.EquipementId });

            builder.Entity<ReservationEquipement>()
                .HasOne(re => re.Reservation)
                .WithMany(r => r.ReservationEquipements)
                .HasForeignKey(re => re.ReservationId);

            builder.Entity<ReservationEquipement>()
                .HasOne(re => re.Equipement)
                .WithMany()
                .HasForeignKey(re => re.EquipementId);
        }
    }
}