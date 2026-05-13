using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.PurchasesController_test
{
    public class PostCompras_test : AppForSEII25264SqliteUT
    {
        private const string _usuarioEmail = "cliente1@test.com";
        private const string _nombreUsuario = "Juan";
        private const string _apellidosUsuario = "García López";
        private const string _direccionEnvio = "Avenida Libertad 45, Barcelona";

        private const string _marcaValida = "Samsung";
        private const string _modeloValido = "Galaxy S24 Ultra";
        private const string _colorValido = "Negro";

        private const string _marcaSinStock = "Sony";
        private const string _modeloSinStock = "Xperia 1 VI";
        private const string _colorSinStock = "Morado";

        private const string _marcaExamen = "Huawei";
        private const string _modeloExamen = "Huawei P70 Pro";
        private const string _colorExamen = "Dorado";



        public PostCompras_test()
        {
            // Datos base: Modelos
            var modelos = new List<Model>
            {
                new Model("Galaxy S24 Ultra"),
                new Model("Xperia 1 VI"),
                new Model("Huawei P70 Pro")
            };

            // Datos base: Dispositivos (uno con stock y otro sin stock)
            var dispositivos = new List<Device>
            {
                new Device(modelos[0], _marcaValida, _colorValido, "Galaxy S24 Ultra 512GB", 1399.99m, 15, 2024),
                new Device(modelos[1], _marcaSinStock, _colorSinStock, "Xperia 1 VI 256GB", 1299.00m, 0, 2024),
                new Device(modelos[2], _marcaExamen, _colorExamen, "Huawei P70 Pro 256GB", 1499.99m, 10, 2025)
            };

            // Datos base: Usuario
            var user = new ApplicationUser("user-cliente-001", _nombreUsuario, _apellidosUsuario, _direccionEnvio, _usuarioEmail)
            {
                UserName = _nombreUsuario,
                Surname = _apellidosUsuario,
                DeliveryAddress = _direccionEnvio
            };

            // Datos base: Compra
            var purchase = new Purchase(
                PaymentMethodTypes.CreditCard,
                DateTime.Now,
                new List<PurchaseItem>(),
                user
            );

            _context.Model.AddRange(modelos);
            _context.Device.AddRange(dispositivos);
            _context.ApplicationUser.AddRange(user);
            _context.SaveChanges();

        }

        public static IEnumerable<object[]> TestCasesFor_CreateCompra()
        {

            // sin items
            var compraSinItems = new purchaseForCreateDTO(_nombreUsuario, _apellidosUsuario, _direccionEnvio, PaymentMethodTypes.CreditCard, 1, // cantidad
            new List<purchaseItemDTO>() // vacío
            );

            // item válido
            var itemsValidos = new List<purchaseItemDTO>()
            {
        new purchaseItemDTO(_marcaValida, _modeloValido, _colorValido, 1399.99m, 1, "Compra válida de prueba")
            };

            //Caso 1:usuario no existe
            var compraUsuarioNoExiste = new purchaseForCreateDTO("Pedro", "Martínez", "Calle Falsa 123, Madrid", PaymentMethodTypes.CreditCard, 1, itemsValidos);
            //Caso 2: dispositivo no existe

            var compraDispositivoNoExiste = new purchaseForCreateDTO(_nombreUsuario, _apellidosUsuario, _direccionEnvio, PaymentMethodTypes.CreditCard, 1,
                new List<purchaseItemDTO>()
                {
                    new purchaseItemDTO("Apple", "iPhone 20 Pro Max", "Blanco", 4399.00m, 1, "No existe en BD")
                }
            );

            //Caso 3: dispositivo sin stock

            var compraDispositivoSinStock = new purchaseForCreateDTO(_nombreUsuario, _apellidosUsuario, _direccionEnvio, PaymentMethodTypes.CreditCard, 1,
                 new List<purchaseItemDTO>()
                 {
                    new purchaseItemDTO(_marcaSinStock, _modeloSinStock, _colorSinStock, 1299.00m, 1, "Sin stock disponible")
                 }
            );

            //Examen
            var compraExamen = new purchaseForCreateDTO(_nombreUsuario, _apellidosUsuario, _direccionEnvio, PaymentMethodTypes.CreditCard, 1,
                 new List<purchaseItemDTO>()
                 {
                    new purchaseItemDTO(_marcaExamen, _modeloExamen, _colorExamen, 1499.99m, 1, "No se puede comprar")
                 }
            );

            var allTests = new List<object[]>
            {
                 new object[] { compraSinItems, "Error. Necesitas seleccionar al menos un dispositivo para ser comprado." },
                 new object[] { compraUsuarioNoExiste, "Error! Usuario no registrado" },
                 new object[] { compraDispositivoNoExiste, "Error! No se encontró el dispositivo" },
                 new object[] { compraDispositivoSinStock, "Error! No hay suficiente stock del dispositivo" },
                 new object[] { compraExamen, "Error! Las tecnologias de estas marcas ya no estan disponibles, siguiendo recomendaciones de las autoridades competentes en materia de seguridad" },
            };

            return allTests;

        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateCompra))]
        public async Task CrearCompra_Error_Test(purchaseForCreateDTO purchaseDTO, string errorEsperado)
        {
            // Arrange
            var mock = new Mock<ILogger<PurchasesController>>();
            ILogger<PurchasesController> logger = mock.Object;

            var controller = new PurchasesController(_context, logger);

            // Act
            var result = await controller.CrearCompra(purchaseDTO);

            // Assert
            // Verificamos que el resultado sea BadRequest
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            // Obtenemos el mensaje de error devuelto
            var errorActual = problemDetails.Errors.First().Value[0];

            // Comparamos que empiece con el esperado (por si contiene detalles adicionales)
            Assert.StartsWith(errorEsperado, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CrearCompra_Success_Test()
        {
            // Arrange
            var mock = new Mock<ILogger<PurchasesController>>();
            ILogger<PurchasesController> logger = mock.Object;

            var controller = new PurchasesController(_context, logger);

            // Creamos una compra válida para el usuario que existe en el contexto
            var purchaseDTO = new purchaseForCreateDTO(
                _nombreUsuario,
                _apellidosUsuario,
                _direccionEnvio,
                PaymentMethodTypes.CreditCard,
                2,
                new List<purchaseItemDTO>()
                {
                    new purchaseItemDTO(_marcaValida, _modeloValido, _colorValido, 1399.99m, 2, "Compra válida de Galaxy S24 Ultra")
                }
            );
            new List<purchaseItemDTO>()
                {
            new purchaseItemDTO(_marcaValida, _modeloValido, _colorValido, 1399.99m, 2, "Compra válida de Galaxy S24 Ultra")
                };

            // DTO esperado de respuesta (CompraDetailDTO)
            var expectedCompraDetailDTO = new purchaseDetailDTO(
                _nombreUsuario,
                _apellidosUsuario,
                _direccionEnvio,
                DateTime.Now,       // fecha compra
                2799.98m,            // 1399.99 * 2
                2,                  // cantidad total
                purchaseDTO.PurchaseItems
            );

            // Act
            var result = await controller.CrearCompra(purchaseDTO);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualCompraDetailDTO = Assert.IsType<purchaseDetailDTO>(createdResult.Value);


            Assert.Equal(expectedCompraDetailDTO, actualCompraDetailDTO);


        }
    }
        /*
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CrearCompra_ThrowsException_Test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PurchasesController>>();
            var controller = new PurchasesController(_context, mockLogger.Object);

            // Forzamos un error: intentamos crear una compra pero manipulamos el contexto 
            // para que falle. Una forma fácil es cerrar la conexión antes de guardar.
            _context.Database.CloseConnection();

            var purchaseDTO = new purchaseForCreateDTO(
                _nombreUsuario, _apellidosUsuario, _direccionEnvio,
                PaymentMethodTypes.CreditCard, DateTime.Now, DateTime.Now,
                new List<purchaseItemDTO> {
            new purchaseItemDTO(_marcaValida, _modeloValido, _colorValido, 100m, 1, "Test")
                }
            );

            // Act
            var result = await controller.CrearCompra(purchaseDTO);

            // Assert
            // Esto debería entrar en el bloque 'catch (Exception ex)'
            Assert.IsType<ConflictObjectResult>(result);
        }
    }
        */
}

