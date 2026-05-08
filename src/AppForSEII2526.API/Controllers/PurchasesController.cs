using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Data;
using AppForSEII2526.API.Models;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            return await _context.Purchases
                .Include(p => p.PurchaseItems)
                .ToListAsync();
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Device)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        // POST: api/Purchases
        [HttpPost]
        public async Task<ActionResult<Purchase>> PostPurchase(PurchaseCreateDTO purchaseDto)
        {
            // 1. Validación de entrada
            if (purchaseDto == null || purchaseDto.Items == null || !purchaseDto.Items.Any())
            {
                return BadRequest("La compra no contiene artículos.");
            }

            // 2. Mapeo del DTO a la Entidad Purchase
            var purchase = new Purchase
            {
                CustomerUserName = purchaseDto.CustomerUserName,
                CustomerUserSurname = purchaseDto.CustomerUserSurname,
                DeliveryAddress = purchaseDto.DeliveryAddress,

                // Intentamos convertir el string a Enum. Si falla, avisamos al usuario.
                PaymentMethod = Enum.TryParse<PaymentMethod>(purchaseDto.PaymentMethod, out var method)
                                ? method : PaymentMethod.CreditCard,

                PurchaseDate = DateTime.Now,
                PurchaseItems = new List<PurchaseItem>(),
                TotalPrice = 0,
                TotalQuantity = 0,
                CustomerId = "Invitado"
            };

            // 3. Procesar cada artículo del DTO
            foreach (var itemDto in purchaseDto.Items)
            {
                var device = await _context.Devices.FindAsync(itemDto.DeviceId);

                if (device == null)
                {
                    return BadRequest($"El dispositivo con ID {itemDto.DeviceId} no existe.");
                }

                var purchaseItem = new PurchaseItem
                {
                    DeviceId = itemDto.DeviceId,
                    Quantity = itemDto.Quantity,
                    PriceAtPurchase = device.priceForPurchase,
                    // CORRECCIÓN ERROR 500: Rellenamos la columna Description que es obligatoria en tu DB
                    Description = $"Dispositivo: {device.Brand} {device.Name}"
                };

                purchase.PurchaseItems.Add(purchaseItem);

                // Actualizamos los totales
                purchase.TotalPrice += (device.priceForPurchase * itemDto.Quantity);
                purchase.TotalQuantity += itemDto.Quantity;
            }

            // 4. Guardar en la base de datos
            try
            {
                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al guardar: {ex.Message}");
            }

            // 5. Respuesta 201 Created
            return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchase);
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}