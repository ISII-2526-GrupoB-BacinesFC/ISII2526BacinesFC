using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDispositivosParaComprar_test : AppForSEII2526.UT.AppForSEII25264SqliteUT
    {
        public GetDispositivosParaComprar_test()
        {
            // === MODELOS ===
            var modelos = new List<Model>
            {
                new Model { Id = 1, Name = "iPhone 14 Pro" },
                new Model { Id = 2, Name = "iPhone 13" },
                new Model { Id = 3, Name = "Galaxy S23 Ultra" },
                new Model { Id = 4, Name = "Galaxy A54" },
                new Model { Id = 5, Name = "Xiaomi 13 Pro" },
                new Model { Id = 6, Name = "Pixel 7 Pro" },
                new Model { Id = 7, Name = "OnePlus 11" },
                new Model { Id = 8, Name = "Huawei P60 Pro" },
                new Model { Id = 9, Name = "Oppo Find X5" },
                new Model { Id = 10, Name = "Realme GT3" }
            };

            // === DISPOSITIVOS ===
            // Ajustado al constructor: (Model, Brand, Color, Name, Price, Quantity, Year)
            var dispositivos = new List<Device>
            {
                // Apple
                new Device(modelos[0], "Apple", "Negro", "iPhone 14 Pro 256GB", 1199.99m, 5, 2023),
                new Device(modelos[0], "Apple", "Plata", "iPhone 14 Pro 512GB", 1399.99m, 3, 2023),
                new Device(modelos[0], "Apple", "Morado", "iPhone 14 Pro 128GB", 1099.99m, 7, 2023),
                new Device(modelos[1], "Apple", "Azul", "iPhone 13 256GB", 799.99m, 10, 2022),
                new Device(modelos[1], "Apple", "Rosa", "iPhone 13 128GB", 699.99m, 12, 2022),
                // Samsung
                new Device(modelos[2], "Samsung", "Verde", "Galaxy S23 Ultra 512GB", 1299.99m, 4, 2023),
                new Device(modelos[2], "Samsung", "Negro", "Galaxy S23 Ultra 256GB", 1099.99m, 6, 2023),
                new Device(modelos[3], "Samsung", "Blanco", "Galaxy A54 5G 256GB", 449.99m, 15, 2023),
                new Device(modelos[3], "Samsung", "Negro", "Galaxy A54 5G 128GB", 399.99m, 18, 2023),

                // Xiaomi
                new Device(modelos[4], "Xiaomi", "Negro", "Xiaomi 13 Pro 256GB", 999.99m, 8, 2023),
                new Device(modelos[4], "Xiaomi", "Blanco", "Xiaomi 13 Pro 512GB", 1099.99m, 5, 2023),
                // Google
                new Device(modelos[5], "Google", "Blanco", "Pixel 7 Pro 256GB", 899.99m, 6, 2022),
                new Device(modelos[5], "Google", "Negro", "Pixel 7 Pro 128GB", 799.99m, 8, 2022),

                // OnePlus
                new Device(modelos[6], "OnePlus", "Verde", "OnePlus 11 5G 256GB", 849.99m, 7, 2023),
                // Huawei
                new Device(modelos[7], "Huawei", "Dorado", "Huawei P60 Pro 256GB", 949.99m, 5, 2023),

                // Oppo
                new Device(modelos[8], "Oppo", "Azul", "Oppo Find X5 Pro 256GB", 799.99m, 6, 2022),

                // Realme
                new Device(modelos[9], "Realme", "Negro", "Realme GT3 240W 256GB", 649.99m, 10, 2023),
                new Device(modelos[9], "Realme", "Blanco", "Realme GT3 240W 128GB", 599.99m, 12, 2023)
            };

            _context.Model.AddRange(modelos);
            _context.Device.AddRange(dispositivos);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDispositivosParaComprar_OK()
        {
            var testCases = new List<object[]>
            {
                // 1. Sin filtros: deben aparecer todos los dispositivos con CantidadParaCompra > 0 (18)
                new object[] { null, null, null, 18 },

                new object[] { null, null, null, 18 },

                // 2. Filtro por nombre “iPhone”
                new object[] { "iPhone", null, null, 5 },

                // 3. Filtro por nombre “Galaxy”
                new object[] { "Galaxy", null, null, 4 },

                // 4. Filtro por nombre “Pixel”
                new object[] { "Pixel", null, null, 2 },

                // 5. Filtro por color “Negro”
                new object[] { null, "Negro", null, 6 },

                // 6. Filtro por nombre “Xiaomi”
                new object[] { "Xiaomi", null, null, 2 },

                // 7. Filtro por nombre “Realme”
                new object[] { "Realme", null, null, 2 },

                // 8. Filtro por nombre “Huawei”
                new object[] { "Huawei", null, null, 1 },

                // 9. Filtro por nombre “OnePlus”
                new object[] { "OnePlus", null, null, 1 },

                // 10. Filtro inexistente
                new object[] { "Nokia", null, null, 0 }
            };

            return testCases;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDispositivosParaComprar_OK))]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDispositivosParaComprar_Filtros_Test(
            string? filtroNombre,
            string? filtroColor,
            float? filtroPrecio,
            int cantidadEsperada)
        {
            // Arrange
            var controller = new DevicesController(_context, null);

            // Act
            var result = await controller.GetDispositivosParaComprar(filtroNombre, filtroColor, filtroPrecio);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dispositivos = Assert.IsType<List<DeviceForPurchaseDTO>>(okResult.Value);

            Assert.Equal(cantidadEsperada, dispositivos.Count);
        }

    }

}