using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AppForSEII2526.API.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger)
        {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try { SeedRoles(roleManager, rolesNames); }
            catch (Exception ex) { logger.LogError(ex, "Error al crear roles."); }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try { SeedUsers(userManager, rolesNames); }
            catch (Exception ex) { logger.LogError(ex, "Error al crear usuarios."); }

            try { SeedModelsAndDevices(dbContext); }
            catch (Exception ex) { logger.LogError(ex, "Error al crear modelos y dispositivos."); }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole { Name = roleName, NormalizedName = roleName.ToUpper() };
                    _ = roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles)
        {
            // 1. ELENA (Tu administradora original - NO SE TOCA)
            string emailElena = "elena@uclm.es";
            if (userManager.FindByNameAsync(emailElena).Result == null)
            {
                ApplicationUser elena = new ApplicationUser
                {
                    UserName = emailElena,
                    Email = emailElena,
                    Name = "Elena",
                    Surname = "Navarro Martínez",
                    DeliveryAddress = "Avda. España 2, Albacete",
                    EmailConfirmed = true
                };
                var result = userManager.CreateAsync(elena, "Password1234%").Result;
                if (result.Succeeded) userManager.AddToRoleAsync(elena, roles[0]).Wait();
            }

            // 2. DATOS DEL SQL: Usuarios de tus colegas
            // Admin del SQL
            SeedUser(userManager, "admin@test.com", "Administrador", "Sistema", "Calle Principal 123, Madrid", roles[0]);
            // Cliente 1 (Juan García López)
            SeedUser(userManager, "cliente1@test.com", "Juan", "García López", "Avenida Libertad 45, Barcelona", roles[2]);
            // Cliente 2 (María Rodríguez Pérez)
            SeedUser(userManager, "cliente2@test.com", "María", "Rodríguez Pérez", "Plaza España 12, Valencia", roles[2]);
        }

        private static void SeedUser(UserManager<ApplicationUser> userManager, string email, string name, string surname, string address, string role)
        {
            if (userManager.FindByNameAsync(email).Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = name,
                    Surname = surname,
                    DeliveryAddress = address,
                    EmailConfirmed = true
                };
                var result = userManager.CreateAsync(user, "Password123!").Result;
                if (result.Succeeded) userManager.AddToRoleAsync(user, role).Wait();
            }
        }

        public static void SeedModelsAndDevices(ApplicationDbContext dbcontext)
        {
            // 1. MODELOS DEL SQL
            if (!dbcontext.Model.Any())
            {
                dbcontext.Model.AddRange(
                    new Model("Galaxy S24 Ultra"), new Model("iPhone 15 Pro Max"),
                    new Model("Pixel 8 Pro"), new Model("ROG Phone 8"),
                    new Model("Xperia 1 VI"), new Model("Redmi Note 13 Pro"),
                    new Model("Surface Duo 2"), new Model("OnePlus 12"),
                    new Model("Motorola Edge 50 Ultra"), new Model("Huawei P60 Pro")
                );
                dbcontext.SaveChanges();
            }

            // 2. DISPOSITIVOS DEL SQL
            if (!dbcontext.Device.Any())
            {
                // Mapeamos los modelos para usarlos en el constructor de Device
                var models = dbcontext.Model.ToList();

                dbcontext.Device.AddRange(
                    new Device(models.Find(m => m.Name == "Galaxy S24 Ultra"), "Samsung", "Negro", "Galaxy S24 Ultra 512GB", 1399.99m, 15, 2024),
                    new Device(models.Find(m => m.Name == "iPhone 15 Pro Max"), "Apple", "Blanco", "iPhone 15 Pro Max 256GB", 1499.00m, 20, 2023),
                    new Device(models.Find(m => m.Name == "Pixel 8 Pro"), "Google", "Gris", "Pixel 8 Pro 128GB", 1099.00m, 10, 2024),
                    new Device(models.Find(m => m.Name == "ROG Phone 8"), "ASUS", "Negro", "ROG Phone 8 16GB", 1199.00m, 8, 2024),
                    new Device(models.Find(m => m.Name == "Xperia 1 VI"), "Sony", "Morado", "Xperia 1 VI 256GB", 1299.00m, 7, 2024),
                    new Device(models.Find(m => m.Name == "Redmi Note 13 Pro"), "Xiaomi", "Azul", "Redmi Note 13 Pro 128GB", 349.99m, 30, 2025),
                    new Device(models.Find(m => m.Name == "Surface Duo 2"), "Microsoft", "Plata", "Surface Duo 2 128GB", 999.00m, 5, 2023),
                    new Device(models.Find(m => m.Name == "OnePlus 12"), "OnePlus", "Verde", "OnePlus 12 256GB", 899.00m, 12, 2025),
                    new Device(models.Find(m => m.Name == "Motorola Edge 50 Ultra"), "Motorola", "Dorado", "Motorola Edge 50 Ultra", 799.00m, 10, 2024),
                    new Device(models.Find(m => m.Name == "Huawei P60 Pro"), "Huawei", "Negro", "Huawei P60 Pro 512GB", 1199.00m, 9, 2023)
                );
                dbcontext.SaveChanges();
            }
        }
    }
}