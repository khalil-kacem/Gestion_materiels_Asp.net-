using Microsoft.EntityFrameworkCore;
using GestionMateriel.Data;
using Microsoft.AspNetCore.Identity;
using GestionMateriel.Models;

var builder = WebApplication.CreateBuilder(args);

// === Configuration de la base de données ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Configuration Identity avec ApplicationUser ===
// === Configuration Identity simplifiée (sans 2FA) ===
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;     // Pas de confirmation email
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Désactiver complètement le 2FA
    options.Tokens.ProviderMap.Clear(); // Optionnel mais propre

    // Paramètres du mot de passe
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// === Pipeline HTTP ===
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();   // Important pour les pages Identity

// === Seed des rôles + admin par défaut ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}
// REDIRECTION AUTOMATIQUE SELON LE RÔLE
// REDIRECTION AUTOMATIQUE SELON LE RÔLE APRÈS CONNEXION
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true &&
        context.Request.Path.Value == "/")
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.User);

        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Administrateur"))
                context.Response.Redirect("/Admin/Index");           // À créer plus tard
            else if (roles.Contains("Responsable"))
                context.Response.Redirect("/Dashboard/Responsable");
            else if (roles.Contains("Enseignant"))
                context.Response.Redirect("/Equipement/Index");
            else
                await next();
            return;
        }
    }
    await next();
});
app.Run();