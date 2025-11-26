using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ecommerce.Persistence
{
    /// <summary>
    /// Esta clase se encarga de "sembrar" (crear si no existen)
    /// los roles básicos de la aplicación (Admin, User)
    /// y de asignar el rol de Admin a un usuario específico.
    /// </summary>
    public static class RoleSeeder
    {
        // Este es el método que llamará Program.cs
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // 1. Obtener los servicios necesarios
            // (Necesitamos IServiceProvider para no causar un ciclo de dependencias)
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 2. Definir los roles que queremos en nuestra app
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                // 3. Crear el rol si NO existe
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 4. Definir tu email de administrador
            // ¡¡¡IMPORTANTE!!! 
            // CAMBIA ESTO POR EL EMAIL REAL QUE USASTE PARA REGISTRARTE
            var adminEmail = "abdallm08@hotmail.com";

            // 5. Buscar al usuario administrador por su email
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // 6. Asignar el rol de "Admin"
            // (Solo si el usuario existe y si NO tiene ya el rol)
            if (adminUser != null)
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}