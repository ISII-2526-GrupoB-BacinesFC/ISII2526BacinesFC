using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ComprasController_test
{
    public class GetCompra : AppForSEII2526.UT.AppForSEII25264SqliteUT
    {
        public GetCompra()
        {

            //CREAR LOS MODELOS NECESARIOS PARA LAS PRUEBAS

            var modelos = new List<Model>()
            {
                new Model { Name = "iPhone 14 Pro" },
                new Model { Name = "Galaxy S23" }
            };

            //CREAR LOS DISPOSITIVOS NECESARIOS PARA LAS PRUEBAS

            var dispositivos = new List<Device>() {

                new Device(modelos[0], "Apple", "Negro", "iPhone 14 Pro 256GB", 1199.99m, 5, 2023),
                new Device(modelos[1], "Samsung", "Blanco", "Galaxy S23 Ultra 512GB", 1299.99m, 8, 2023),


            };

            _context.AddRange(modelos);
            _context.AddRange(dispositivos);
            _context.SaveChanges();

            // Crear usuario
            ApplicationUser user = new ApplicationUser(
                "0",
                "Juan",
                "Pérez García",
                "Calle Mayor 123, Madrid",
                "Juanito27@gmail.com"
            );
            user.Email = "juan.perez@email.com";
            user.UserName = "Juan";

            // Crear compra
            var compra = new Purchase(
                PaymentMethodTypes.CreditCard,
                DateTime.Now,
                new List<PurchaseItem>(),
                user
            );

            // Crear items de compra
            var itemCompra1 = new PurchaseItem(dispositivos[0].Id, dispositivos[0].PriceForPurchase, 2);
            itemCompra1.Description = "iPhone 14 Pro 256GB, me encanta";

            var itemCompra2 = new PurchaseItem(dispositivos[1].Id, dispositivos[1].PriceForPurchase, 1);
            itemCompra2.Description = "Galaxy S23 Ultra 512GB, es para mi padre";

            compra.PurchaseItems.Add(itemCompra1);
            compra.PurchaseItems.Add(itemCompra2);

            // Calcular totales
            compra.TotalPrice = (dispositivos[0].PriceForPurchase * 2) + (dispositivos[1].PriceForPurchase * 1);
            compra.TotalQuantity = 3;

            _context.Users.Add(user);
            _context.Add(compra);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDetalleCompra_NotFound_Test()
        {
            // Arrange
            var mock = new Mock<ILogger<PurchasesController>>();
            ILogger<PurchasesController> logger = mock.Object;
            var controller = new PurchasesController(_context, logger);

            // Act
            var result = await controller.GetPurchaseDetail(0);

            // Assert
            // Verificamos que el response type es NotFound
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDetalleCompra_Found_Test()
        {
            // Arrange
            var mock = new Mock<ILogger<PurchasesController>>();
            var controller = new PurchasesController(_context, mock.Object);

            // 1. SACAMOS LOS DATOS REALES DE LA DB
            // Esto es vital: así el test conoce el ID y el UserName exacto que generó SQLite
            var compraEnDB = _context.Purchase.Include(c => c.ApplicationUser).First();

            // 2. CONSTRUIMOS EL ESPERADO USANDO ESOS DATOS REALES
            var expectedCompra = new purchaseDetailDTO(
                compraEnDB.ApplicationUser.UserName, // <--- Evita el error "Juan" vs "juan.perez@email.com"
                compraEnDB.ApplicationUser.Surname,
                compraEnDB.ApplicationUser.DeliveryAddress,
                compraEnDB.PurchaseDate,
                3699.97m,
                3,
                new List<purchaseItemDTO>()
            )
            {
                Id = compraEnDB.Id // <--- ESTO ARREGLA EL ERROR DEL ID (0 vs 1)
            };

            // Añadimos los items manualmente al esperado para comparar la lista completa
            expectedCompra.PurchaseItems.Add(new purchaseItemDTO("Apple", "iPhone 14 Pro", "Negro", 1199.99m, 2, "iPhone 14 Pro 256GB, me encanta"));
            expectedCompra.PurchaseItems.Add(new purchaseItemDTO("Samsung", "Galaxy S23", "Blanco", 1299.99m, 1, "Galaxy S23 Ultra 512GB, es para mi padre"));

            // Act
            // Usamos el ID real que tiene la base de datos
            var result = await controller.GetPurchaseDetail(compraEnDB.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var compraDTOActual = Assert.IsType<purchaseDetailDTO>(okResult.Value);

            // Ahora, gracias al Equals() que ya tienes en el DTO, esto debería dar VERDE
            Assert.Equal(expectedCompra, compraDTOActual);
        }
    }
}