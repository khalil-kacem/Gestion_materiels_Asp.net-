using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using GestionMateriel.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionMateriel.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Création des rôles
            string[] roleNames = { "Administrateur", "Responsable", "Enseignant" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin par défaut
            var admin = await userManager.FindByEmailAsync("admin@gestion.edu");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@gestion.edu",
                    Email = "admin@gestion.edu",
                    NomComplet = "Administrateur Système",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Administrateur");
            }

            // Responsable
            var responsable = await userManager.FindByEmailAsync("responsable@gestion.edu");
            if (responsable == null)
            {
                responsable = new ApplicationUser
                {
                    UserName = "responsable@gestion.edu",
                    Email = "responsable@gestion.edu",
                    NomComplet = "Ahmed Benali - Responsable Matériel",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(responsable, "Resp123!");
                await userManager.AddToRoleAsync(responsable, "Responsable");
            }

            // Enseignant
            var enseignant = await userManager.FindByEmailAsync("enseignant@gestion.edu");
            if (enseignant == null)
            {
                enseignant = new ApplicationUser
                {
                    UserName = "enseignant@gestion.edu",
                    Email = "enseignant@gestion.edu",
                    NomComplet = "Fatma Trabelsi - Enseignante",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(enseignant, "Ens123!");
                await userManager.AddToRoleAsync(enseignant, "Enseignant");
            }

            // Catégories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Categorie { Nom = "Ordinateurs Portables", Description = "PC et MacBooks" },
                    new Categorie { Nom = "Projecteurs", Description = "Projecteurs HDMI / VGA" },
                    new Categorie { Nom = "Tablettes & iPad", Description = "Tablettes éducatives" },
                    new Categorie { Nom = "Matériel Audio", Description = "Micros, enceintes, casques" },
                    new Categorie { Nom = "Écrans & Moniteurs", Description = "Écrans 24\" et 27\"" }
                );
                await context.SaveChangesAsync();
            }

            // Équipements de test
            if (!context.Equipements.Any())
            {
                var catOrdi = await context.Categories.FirstOrDefaultAsync(c => c.Nom == "Ordinateurs Portables");
                var catProj = await context.Categories.FirstOrDefaultAsync(c => c.Nom == "Projecteurs");

                if (catOrdi != null && catProj != null)
                {
                    context.Equipements.AddRange(
                        new Equipement { Nom = "Dell Latitude 5520", CategorieId = catOrdi.Id, EstDisponible = true, Description = "i5 - 16Go RAM" },
                        new Equipement { Nom = "HP EliteBook 840 G8", CategorieId = catOrdi.Id, EstDisponible = true, Description = "i7 - 32Go RAM" },
                        new Equipement { Nom = "Epson EB-FH06", CategorieId = catProj.Id, EstDisponible = false, Description = "Full HD 4000 lumens" },
                        new Equipement { Nom = "iPad Air 5 (2022)", CategorieId = context.Categories.First(c => c.Nom == "Tablettes & iPad").Id, EstDisponible = true }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}