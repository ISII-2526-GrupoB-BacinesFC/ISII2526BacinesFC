using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Data;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Inyección de dependencias para acceder a la DB
        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceForPurchaseDTO>>> GetDevicesForPurchase()
        {
            // Eager loading con Include y proyección a DTO con Select
            return await _context.Devices
                .Include(d => d.Model)
                .Select(d => new DeviceForPurchaseDTO(
                    d.Id,
                    d.Brand,
                    d.Name,
                    d.Model != null ? d.Model.NameModel : "Sin modelo",
                    d.Color,
                    d.priceForPurchase
                ))
                .ToListAsync();
        }
    }
}