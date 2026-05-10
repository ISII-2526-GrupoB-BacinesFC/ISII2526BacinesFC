using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchasesController> _logger;

        public PurchasesController(ApplicationDbContext context, ILogger<PurchasesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(purchaseDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPurchaseDetail(int id)
        {
            if (_context.Purchase == null)
            {
                _logger.LogError("Error: La tabla Purchase no existe");
                return NotFound();
            }

            // Nos traemos la compra con sus items y los dispositivos relacionados
            var purchase = await _context.Purchase
                .Include(c => c.ApplicationUser)
                .Include(c => c.PurchaseItems)
                    .ThenInclude(d => d.Device)
                        .ThenInclude(m => m.Model)
                .FirstOrDefaultAsync(c => c.Id == id);

            // Comprobamos que la compra existe
            if (purchase == null)
            {
                _logger.LogError($"Error: Compra con {id} no existe");
                return NotFound();
            }

            // Tenemos que crear los PurchaseItemDTO manualmente en memoria pq SQLite no lo soprta y no lo traducimos a SQL
            var purchaseDTO = new purchaseDetailDTO(
                purchase.ApplicationUser.UserName,
                purchase.ApplicationUser.Surname,
                purchase.ApplicationUser.DeliveryAddress,
                purchase.PurchaseDate,
                Math.Round(purchase.TotalPrice, 2),
                purchase.TotalQuantity,
                purchase.PurchaseItems
                    .Select(ci => new purchaseItemDTO(
                        ci.Device.Brand,
                        ci.Device.Model.Name,
                        ci.Device.Color,
                        Math.Round((double)ci.Device.PriceForPurchase, 2),
                        ci.Quantity, // Seguimos usando la cantidad total de items en la compra
                        ci.Description
                    ))
                    .ToList()
            )
            {
                Id = purchase.Id
            };


            return Ok(purchaseDTO);
        }



        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(purchaseDetailDTO), (int)HttpStatusCode.Created)]
        public async Task<ActionResult> CrearCompra(purchaseForCreateDTO purchaseForCreate)
        {
            // Validar que hay items en la compra
            if (purchaseForCreate.PurchaseItems == null || purchaseForCreate.PurchaseItems.Count == 0)
            {
                ModelState.AddModelError("PurchaseItems", "Error. Necesitas seleccionar al menos un dispositivo para ser comprado.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Buscar el usuario
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == purchaseForCreate.CustomerUserName
                                       && u.Surname == purchaseForCreate.CustomerNameSurname);

            if (user == null)
            {
                ModelState.AddModelError("PurchaseApplicationUser", "Error! Usuario no registrado");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Crear la compra (sin ItemsCompra inicialmente)
            var purchase = new Purchase(
                purchaseForCreate.PaymentMethod,
                purchaseForCreate.PurchaseDateFrom,
                new List<PurchaseItem>(),
                user
            );

            double totalPrice = 0;
            int totalQuantity = 0;

            // Procesar cada item de la compra
            foreach (var itemDTO in purchaseForCreate.PurchaseItems)
            {
                // Buscar el dispositivo por marca, modelo y color
                var device = await _context.Device
                    .Include(d => d.Model)
                    .Where(d => d.Brand == itemDTO.Brand    
                             && d.Model.Name == itemDTO.Model
                             && d.Color == itemDTO.Color)
                    .FirstOrDefaultAsync();

                if (device == null)
                {
                    ModelState.AddModelError("DispositivoNoExiste",
                        $"Error! No se encontró el dispositivo: Marca='{itemDTO.Brand}', Modelo='{itemDTO.Model}', Color='{itemDTO.Color}'");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                // Validar stock disponible
                if (device.QuantityForPurchase < itemDTO.Quantity)
                {
                    ModelState.AddModelError("DispositivoNoDisponible",
                        $"Error! No hay suficiente stock del dispositivo '{itemDTO.Model}'. Disponible: {device.QuantityForPurchase}, Solicitado: {itemDTO.Quantity}");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                // Validar marca
                if (itemDTO.Brand.Contains("Xiaomi") || itemDTO.Brand.Contains("Huawei"))
                {
                    ModelState.AddModelError("TecnologiaNoDisponible",
                        $"Error! Las tecnologias de estas marcas ya no estan disponibles, siguiendo recomendaciones de las autoridades competentes en materia de seguridad");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                // Actualizar la cantidad disponible del dispositivo
                device.QuantityForPurchase -= itemDTO.Quantity;

                // Crear el ItemCompra - SOLO con el constructor básico
                var purchaseItem = new PurchaseItem
                {
                    Device = device, // Establecer la relación con el dispositivo
                    Quantity = itemDTO.Quantity,
                    Price = (decimal)device.PriceForPurchase
                };

                // NO establecer manualmente IdDispositivo ni Dispositivo
                // Entity Framework lo hará automáticamente por la relación

                purchaseItem.Description = itemDTO.Description;

                // Agregar el item a la compra
                purchase.PurchaseItems.Add(purchaseItem);

                // Calcular totales
                totalPrice += (double)device.PriceForPurchase * itemDTO.Quantity;
                totalQuantity += itemDTO.Quantity;
                // Actualizar el precio en el DTO para la respuesta
                itemDTO.Price = (double)device.PriceForPurchase;
            }

            // Establecer los totales en la compra
            purchase.TotalPrice = totalPrice;
            purchase.TotalQuantity = totalQuantity;

            // Verificar si hay errores de validación
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Log para debugging
            _logger.LogInformation($"Creando compra con {purchase.PurchaseItems.Count} items");
            foreach (var item in purchase.PurchaseItems)
            {
                _logger.LogInformation($"ItemCompra: DispositivoId={item.DeviceId}, Cantidad={item.Quantity}, Precio={item.Price}");
            }

            // Agregar la compra al contexto
            _context.Add(purchase);
            try
            {
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? "No inner exception";
                _logger.LogError($"{DateTime.Now}: {ex.ToString()}");
                _logger.LogError($"Inner Exception: {innerException}");
                return Conflict($"Error al guardar la compra: {ex.Message}. Inner: {innerException}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now}: {ex.ToString()}");
                return Conflict("Error al guardar la compra: " + ex.Message);
            }

            // Crear el DTO de respuesta
            // SUSTITUYE EL BLOQUE DE "Crear el DTO de respuesta" POR ESTE:

            var purchaseDetail = new purchaseDetailDTO(
                purchase.ApplicationUser.UserName,
                purchase.ApplicationUser.Surname,
                purchase.ApplicationUser.DeliveryAddress,
                purchase.PurchaseDate,
                purchase.TotalPrice,
                purchase.TotalQuantity,
                purchase.PurchaseItems.Select(pi => new purchaseItemDTO(
                    pi.Device.Brand,
                    pi.Device.Model.Name,
                    pi.Device.Color,
                    (double)pi.Price, // Hacemos el cast a double si tu DTO usa double
                    pi.Quantity,
                    pi.Description
                )).ToList()
            )
            {
                Id = purchase.Id
            };

            return CreatedAtAction("GetPurchaseDetail", new { id = purchase.Id }, purchaseDetail);
        }


    }
}