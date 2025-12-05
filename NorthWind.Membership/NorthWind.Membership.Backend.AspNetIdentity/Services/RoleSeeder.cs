using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NorthWind.Membership.Backend.AspNetIdentity.Entities;

namespace NorthWind.Membership.Backend.AspNetIdentity.Services
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAndSuperUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<NorthWindUser>>();

            // CAMBIO: Agregamos "Customer" aquí también para asegurar consistencia
            string[] roles = { "SuperUser", "Administrator", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear SuperUser si no existe
            var superUserEmail = "superuser@northwind.com";
            var superUser = await userManager.FindByEmailAsync(superUserEmail);

            if (superUser == null)
            {
                superUser = new NorthWindUser
                {
                    UserName = superUserEmail,
                    Email = superUserEmail,
                    FirstName = "Super",
                    LastName = "User",
                    Cedula = "1234567890",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(superUser, "SuperUser123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superUser, "SuperUser");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(superUser, "SuperUser"))
                {
                    await userManager.AddToRoleAsync(superUser, "SuperUser");
                }
            }
        }
    }
}