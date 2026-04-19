using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMateriel.Data;
using GestionMateriel.Models;

namespace GestionMateriel.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Enseignant : Mes réservations
        [Authorize(Roles = "Enseignant")]
        public async Task<IActionResult> MesReservations()
        {
            var userId = _userManager.GetUserId(User);
            var reservations = await _context.Reservations
                .Include(r => r.ReservationEquipements)
                .ThenInclude(re => re.Equipement)
                .Where(r => r.EnseignantId == userId)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return View(reservations);
        }

        // Responsable : Toutes les réservations
        [Authorize(Roles = "Responsable")]
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Enseignant)
                .Include(r => r.ReservationEquipements)
                .ThenInclude(re => re.Equipement)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return View(reservations);
        }

        // Formulaire de réservation (Enseignant)
        [Authorize(Roles = "Enseignant")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Equipements = await _context.Equipements
                .Where(e => e.EstDisponible)
                .Include(e => e.Categorie)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Enseignant")]
        public async Task<IActionResult> Create([FromForm] int[] equipementIds, DateTime dateDebut, DateTime dateRetourPrevue, string? commentaire)
        {
            if (equipementIds == null || equipementIds.Length == 0)
            {
                ModelState.AddModelError("", "Vous devez sélectionner au moins un équipement.");
                ViewBag.Equipements = await _context.Equipements.Where(e => e.EstDisponible).ToListAsync();
                return View();
            }

            var userId = _userManager.GetUserId(User);

            var reservation = new Reservation
            {
                EnseignantId = userId,
                DateDebut = dateDebut,
                DateRetourPrevue = dateRetourPrevue,
                Commentaire = commentaire,
                Statut = "En attente"
            };

            foreach (var eqId in equipementIds)
            {
                reservation.ReservationEquipements.Add(new ReservationEquipement
                {
                    EquipementId = eqId
                });
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Votre réservation a été envoyée et est en attente d'approbation.";
            return RedirectToAction(nameof(MesReservations));
        }

        // Approbation / Refus par le Responsable
        [Authorize(Roles = "Responsable")]
        [HttpPost]
        public async Task<IActionResult> Approuver(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Statut = "Approuvée";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Réservation approuvée avec succès.";
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Responsable")]
        [HttpPost]
        public async Task<IActionResult> Refuser(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Statut = "Refusée";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Réservation refusée.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}