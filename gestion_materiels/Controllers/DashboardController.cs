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
            var reservationsEnAttente = await _context.Reservations.CountAsync(r => r.Statut == "En attente");
            var rapportsNonLus = await _context.Rapports.CountAsync(r => !r.EstLu);

            var materielPlusReserve = await _context.ReservationEquipements
                .GroupBy(re => re.EquipementId)
                .Select(g => new
                {
                    NomEquipement = g.First().Equipement!.Nom,
                    Nombre = g.Count()
                })
                .OrderByDescending(x => x.Nombre)
                .FirstOrDefaultAsync();

            ViewBag.ReservationsEnAttente = reservationsEnAttente;
            ViewBag.RapportsNonLus = rapportsNonLus;
            ViewBag.MaterielPlusReserve = materielPlusReserve?.NomEquipement ?? "Aucun";
            ViewBag.NombreMax = materielPlusReserve?.Nombre ?? 0;

            return View();
        }
    }
}