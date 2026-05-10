using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AppForSEII2526.API.Data
{
    public static class SeedData
    {

        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger)
        {
            // Se mantienen los roles originales de la plantilla
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try
            {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try
            {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }

            // NUEVO: Cargamos tus modelos de dispositivos y los dispositivos propiamente dichos
            try
            {
                SeedModelsAndDevices(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding Models and Devices in the Database.");
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
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles)
        {
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null)
            {
                // He adaptado el constructor de ApplicationUser según lo que pusiste antes
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es", "Avda. España 2", "Albacete");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    // Asignamos el rol de Administrador (el primero de la lista)
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }
        }

        public static void SeedModelsAndDevices(ApplicationDbContext dbcontext)
        {
            // 1. Crear los "Model" (categorías de dispositivos)
            string[] modelNames = { "Smartphone", "Tablet", "Laptop", "Smartwatch" };
            List<Model> models = new List<Model>();

            foreach (string name in modelNames)
            {
                var model = dbcontext.Model.FirstOrDefault(m => m.Name == name);
                if (model == null)
                {
                    var newModel = new Model(name);
                    dbcontext.Model.Add(newModel);
                    models.Add(newModel);
                }
                else
                {
                    models.Add(model);
                }
            }

            // Guardamos para tener los IDs de los modelos
            dbcontext.SaveChanges();

            // 2. Crear los "Device" reales
            if (!dbcontext.Device.Any(d => d.Name == "iPhone 15 Pro"))
            {
                var iphone = new Device(
                    models.First(m => m.Name == "Smartphone"),
                    "Apple", "Negro", "iPhone 15 Pro",
                    1200, // Usando double
                    10, 2024
                );
                dbcontext.Device.Add(iphone);
            }

            if (!dbcontext.Device.Any(d => d.Name == "Galaxy S24"))
            {
                var samsung = new Device(
                    models.First(m => m.Name == "Smartphone"),
                    "Samsung", "Gris", "Galaxy S24",
                    950, // Usando double
                    15, 2024
                );
                dbcontext.Device.Add(samsung);
            }

            if (!dbcontext.Device.Any(d => d.Name == "iPad Air"))
            {
                var ipad = new Device(
                    models.First(m => m.Name == "Tablet"),
                    "Apple", "Azul", "iPad Air",
                    700, // Usando double
                    8, 2023
                );
                dbcontext.Device.Add(ipad);
            }

            // Guardamos todo en la base de datos
            dbcontext.SaveChanges();
        }
    }
}