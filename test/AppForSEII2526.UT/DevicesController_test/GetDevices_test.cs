using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.Models;
using Xunit;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDevices_test : AppForSEII25264SqliteUT
    {
        public GetDevices_test() : base()
        {
            var devices = new List<Device>
            {
                new Device { Id = 1, Brand = "Apple", Name = "iPhone 15", priceForPurchase = 1000 },
                new Device { Id = 2, Brand = "Samsung", Name = "S24", priceForPurchase = 900 }
            };

            _context.Devices.AddRange(devices);
            _context.SaveChanges();
        }
        
        [Fact]
        public async Task GetDevices_ReturnsAllDevices_test()
        {
            var controller = new DevicesController(_context);

            var result = await controller.GetDevicesForPurchase();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Device>>>(result);
            var devices = Assert.IsAssignableFrom<IEnumerable<Device>>(actionResult.Value);

            Assert.Equal(2, devices.Count());
        }
    }
}