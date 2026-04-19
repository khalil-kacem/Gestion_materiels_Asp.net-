using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMateriel.Data;
using GestionMateriel.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GestionMateriel.Controllers
{
    [Authorize(Roles = "Responsable")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Tableau de bord principal du Responsable
        public async Task<IActionResult> Responsable()
        {
            // 1. Réservations en attente
            var reservationsEnAttente = await _context.Reservations
                .CountAsync(r => r.Statut == "En attente");

            // 2. Rapports non lus
            var rapportsNonLus = await _context.Rapports
                .CountAsync(r => !r.EstLu);

            // 3. Matériel le plus réservé
            var materielPlusReserve = await _context.ReservationEquipements
                .GroupBy(re => re.EquipementId)
                .Select(g => new
                {
                    EquipementId = g.Key,
                    NomEquipement = g.First().Equipement!.Nom,
                    NombreReservations = g.Count()
                })
                .OrderByDescending(x => x.NombreReservations)
                .FirstOrDefaultAsync();

            // Statistiques supplémentaires
            var totalEquipements = await _context.Equipements.CountAsync();
            var equipementsDisponibles = await _context.Equipements.CountAsync(e => e.EstDisponible);

            ViewBag.ReservationsEnAttente = reservationsEnAttente;
            ViewBag.RapportsNonLus = rapportsNonLus;
            ViewBag.TotalEquipements = totalEquipements;
            ViewBag.EquipementsDisponibles = equipementsDisponibles;
            ViewBag.MaterielPlusReserve = materielPlusReserve?.NomEquipement ?? "Aucun";
            ViewBag.NombreReservationsMax = materielPlusReserve?.NombreReservations ?? 0;

            return View();
        }
    }
}