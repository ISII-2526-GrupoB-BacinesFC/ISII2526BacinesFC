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
            // Cargamos las líneas de compra para que no salgan nulas
            return await _context.Purchases
                .Include(p => p.PurchaseItems)
                .ToListAsync();
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            // Cargamos la compra con sus líneas y los datos del dispositivo
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
            // 1. Validación de seguridad
            if (purchaseDto == null || purchaseDto.Items == null || !purchaseDto.Items.Any())
            {
                return BadRequest("La compra no tiene artículos o el formato es incorrecto.");
            }

            // 2. Mapeo inicial (DTO -> Entidad)
            var purchase = new Purchase
            {
                CustomerUserName = purchaseDto.CustomerUserName,
                CustomerUserSurname = purchaseDto.CustomerUserSurname,
                DeliveryAddress = purchaseDto.DeliveryAddress,
                PurchaseDate = DateTime.Now,
                PurchaseItems = new List<PurchaseItem>(),
                TotalPrice = 0,
                TotalQuantity = 0,
                CustomerId = "Invitado" // O el ID que gestiones internamente
            };

            // Intentar convertir el texto del método de pago al Enum del modelo
            if (Enum.TryParse<PaymentMethod>(purchaseDto.PaymentMethod, out var method))
            {
                purchase.PaymentMethod = method;
            }

            // 3. Procesar cada artículo y calcular precios/cantidades
            foreach (var itemDto in purchaseDto.Items)
            {
                // Buscamos el dispositivo para tener el precio real de la DB
                var device = await _context.Devices.FindAsync(itemDto.DeviceId);

                if (device == null)
                {
                    return BadRequest($"El dispositivo con ID {itemDto.DeviceId} no existe en la base de datos.");
                }

                // Creamos la línea de detalle (PurchaseItem)
                var purchaseItem = new PurchaseItem
                {
                    DeviceId = itemDto.DeviceId,
                    Quantity = itemDto.Quantity,
                    PriceAtPurchase = device.priceForPurchase,
                    // IMPORTANTE: Rellenamos la descripción para evitar error 500 en DB
                    Description = $"Compra de {device.Brand} {device.Name}"
                };

                // Añadimos a la lista y actualizamos totales
                purchase.PurchaseItems.Add(purchaseItem);
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
                // Si algo falla en la DB, devolvemos un error detallado
                return StatusCode(500, $"Error al guardar la compra: {ex.InnerException?.Message ?? ex.Message}");
            }

            // 5. Respuesta 201 Created según el estándar
            return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchase);
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}