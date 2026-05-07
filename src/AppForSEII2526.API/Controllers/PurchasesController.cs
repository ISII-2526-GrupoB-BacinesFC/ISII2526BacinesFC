using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Data;
using AppForSEII2526.API.Models;
using AppForSEII2526.API.DTOs; // Importante para reconocer tus DTOs

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
        // Devuelve todas las compras (opcional, pero útil para debugear)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            return await _context.Purchases
                .Include(p => p.PurchaseItems)
                .ToListAsync();
        }

        // GET: api/Purchases/5
        // Requisito: Get Details (GetPurchase)
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            // Cargamos la compra con sus líneas y los datos del dispositivo asociado
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
        // Requisito: Post (CreatePurchase)
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
                CustomerId = purchaseDto.CustomerId,
                PurchaseDate = DateTime.Now,
                PurchaseItems = new List<PurchaseItem>(),
                TotalPrice = 0 // Se calculará dinámicamente
            };

            // 3. Procesar cada artículo del DTO
            foreach (var itemDto in purchaseDto.Items)
            {
                // Buscamos el dispositivo para obtener el precio real de la DB
                var device = await _context.Devices.FindAsync(itemDto.DeviceId);

                if (device == null)
                {
                    return BadRequest($"El dispositivo con ID {itemDto.DeviceId} no existe.");
                }

                // Creamos la línea de detalle (PurchaseItem)
                var purchaseItem = new PurchaseItem
                {
                    DeviceId = itemDto.DeviceId,
                    Quantity = itemDto.Quantity,
                    PriceAtPurchase = device.priceForPurchase // Fijamos el precio del momento de compra
                };

                // Añadimos la línea a la compra y actualizamos el total
                purchase.PurchaseItems.Add(purchaseItem);
                purchase.TotalPrice += (device.priceForPurchase * itemDto.Quantity);
            }

            // 4. Guardar en la base de datos
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // 5. Respuesta 201 Created con el objeto final
            return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchase);
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}