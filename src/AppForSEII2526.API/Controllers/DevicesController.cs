using AppForSEII2526.API.DTOs.DeviceDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ApplicationDbContext context,
            ILogger<DevicesController > logger)
        {
            _context = context;
            _logger = logger;
        }



        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceForPurchaseDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDispositivosParaComprar(string? filtroNombre, string? filtroColor, float? filtroPrecio)
        {
            var device = await _context.Device
                .Where(d => (d.Name.Contains(filtroNombre) || filtroNombre == null) && (d.Color.Contains(filtroColor) || filtroColor == null) && (d.PriceForPurchase.Equals(filtroPrecio) || filtroPrecio == null))
                .Select(d => new DeviceForPurchaseDTO (
                    d.Id,
                    d.Name,
                    d.Brand,
                    d.Model,
                    d.Color,
                    d.PriceForPurchase
                ))
                .ToListAsync();
            return Ok(device);
        }
    }
}