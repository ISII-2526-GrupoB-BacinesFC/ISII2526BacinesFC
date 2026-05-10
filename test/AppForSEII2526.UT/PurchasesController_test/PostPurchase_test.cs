using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.Models;
using AppForSEII2526.API.DTOs;
using Xunit;

namespace AppForSEII2526.UT.PurchasesController_test
{
    public class PostPurchase_test : AppForSEII25264SqliteUT
    {
        public PostPurchase_test()
        {
            // Arreglar: Metemos un dispositivo para poder comprarlo en los tests
            var device = new Device
            {
                Id = 1,
                Brand = "Apple",
                Name = "iPhone 15",
                priceForPurchase = 1000,
                quantityForPurchase = 10
            };
            _context.Devices.Add(device);
            _context.SaveChanges();
        }

        [Fact]
        public async Task PostPurchase_Success_test()
        {
            // Arrange (Preparar)
            var controller = new PurchasesController(_context);
            var purchaseDto = new PurchaseCreateDTO
            {
                CustomerUserName = "Pepe",
                CustomerUserSurname = "Viyuela",
                DeliveryAddress = "Calle Falsa 123",
                PaymentMethod = "CreditCard",
                Items = new List<PurchaseItemCreateDTO>
                {
                    new PurchaseItemCreateDTO { DeviceId = 1, Quantity = 1 }
                }
            };

            // Act (Actuar)
            var result = await controller.PostPurchase(purchaseDto);

            // Assert (Afirmar/Verificar)
            var createdResult = Assert.IsType<CreatedAtActionResult>(result); // Verificamos que es un 201 [cite: 1430]
            var purchaseActual = Assert.IsType<Purchase>(createdResult.Value);
            Assert.Equal(1000, purchaseActual.TotalPrice); // Verificamos que el cálculo es correcto
        }
    }
}
