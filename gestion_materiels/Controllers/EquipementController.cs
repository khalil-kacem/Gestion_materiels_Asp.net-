using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMateriel.Data;
using GestionMateriel.Models;

namespace GestionMateriel.Controllers
{
    [Authorize]  // Tous les rôles connectés peuvent voir la liste
    public class EquipementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Liste des équipements avec filtrage par catégorie
        public async Task<IActionResult> Index(string? categorieFilter = null)
        {
            var query = _context.Equipements
                .Include(e => e.Categorie)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categorieFilter))
            {
                query = query.Where(e => e.Categorie!.Nom == categorieFilter);
            }

            var equipements = await query.ToListAsync();
            ViewBag.Categories = await _context.Categories.Select(c => c.Nom).Distinct().ToListAsync();
            ViewBag.CategorieFilter = categorieFilter;

            return View(equipements);
        }

        // Formulaire de création
        [Authorize(Roles = "Responsable,Administrateur")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Responsable,Administrateur")]
        public async Task<IActionResult> Create(Equipement equipement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipement);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Équipement ajouté avec succès !";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(equipement);
        }

        // Détails
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var equipement = await _context.Equipements
                .Include(e => e.Categorie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipement == null) return NotFound();

            return View(equipement);
        }

        // Modification
        [Authorize(Roles = "Responsable,Administrateur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var equipement = await _context.Equipements.FindAsync(id);
            if (equipement == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(equipement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Responsable,Administrateur")]
        public async Task<IActionResult> Edit(int id, Equipement equipement)
        {
            if (id != equipement.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipement);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Équipement modifié avec succès !";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipementExists(equipement.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(equipement);
        }

        // Suppression avec protection
        [Authorize(Roles = "Responsable,Administrateur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var equipement = await _context.Equipements
                .Include(e => e.Categorie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipement == null) return NotFound();

            // Vérification : ne pas supprimer si réservé
            if (equipement.EstReserve || !equipement.EstDisponible)
            {
                TempData["Error"] = "Impossible de supprimer cet équipement car il est actuellement réservé ou en utilisation.";
                return RedirectToAction(nameof(Index));
            }

            return View(equipement);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Responsable,Administrateur")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipement = await _context.Equipements.FindAsync(id);
            if (equipement != null)
            {
                _context.Equipements.Remove(equipement);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Équipement supprimé avec succès.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EquipementExists(int id)
        {
            return _context.Equipements.Any(e => e.Id == id);
        }
    }
}