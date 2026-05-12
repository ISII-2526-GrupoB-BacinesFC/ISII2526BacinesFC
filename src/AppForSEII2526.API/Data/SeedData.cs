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
            try
            {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the roles.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try
            {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Users.");
            }

            try
            {
                SeedModelsAndDevices(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding Models and Devices.");
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    };
                    _ = roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles)
        {
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null)
            {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "Avda. España 2, Albacete", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }
        }

        public static void SeedModelsAndDevices(ApplicationDbContext dbcontext)
        {
            // 1. Asegurar que existen los Modelos
            if (!dbcontext.Model.Any())
            {
                dbcontext.Model.AddRange(
                    new Model("Smartphone"),
                    new Model("Tablet"),
                    new Model("Laptop")
                );
                dbcontext.SaveChanges();
            }

            // Recuperamos los modelos de la DB para que EF los tenga traqueados
            var smartphoneModel = dbcontext.Model.First(m => m.Name == "Smartphone");
            var tabletModel = dbcontext.Model.First(m => m.Name == "Tablet");

            // 2. Crear los Devices si no existen
            if (!dbcontext.Device.Any())
            {
                dbcontext.Device.AddRange(
                    new Device(smartphoneModel, "Apple", "Negro", "iPhone 15 Pro", 1200.0m, 10, 2024),
                    new Device(smartphoneModel, "Samsung", "Gris", "Galaxy S24", 950.0m, 15, 2024),
                    new Device(tabletModel, "Apple", "Azul", "iPad Air", 700.0m, 8, 2023)
                );

                dbcontext.SaveChanges();
            }
        }
    }
}