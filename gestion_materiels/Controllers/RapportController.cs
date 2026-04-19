using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMateriel.Data;
using GestionMateriel.Models;

namespace GestionMateriel.Controllers
{
    public class RapportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RapportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Responsable : Liste de tous les rapports
        [Authorize(Roles = "Responsable")]
        public async Task<IActionResult> Index()
        {
            var rapports = await _context.Rapports
                .Include(r => r.Reservation)
                .ThenInclude(res => res!.Enseignant)
                .OrderByDescending(r => r.DateRapport)
                .ToListAsync();

            return View(rapports);
        }

        // Marquer un rapport comme lu
        [Authorize(Roles = "Responsable")]
        [HttpPost]
        public async Task<IActionResult> MarquerCommeLu(int id)
        {
            var rapport = await _context.Rapports.FindAsync(id);
            if (rapport != null)
            {
                rapport.EstLu = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}