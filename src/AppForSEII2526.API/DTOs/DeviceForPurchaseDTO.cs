using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs
{
    public class DeviceForPurchaseDTO
    {
        // Constructor para proyectar directamente desde LINQ
        public DeviceForPurchaseDTO(int id, string brand, string name, string modelName, string color, double price)
        {
            Id = id;
            Brand = brand;
            Name = name;
            ModelName = modelName;
            Color = color;
            PriceForPurchase = price;
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public string ModelName { get; set; } // Propiedad "aplanada" del modelo relacionado

        public string Color { get; set; }

        public double PriceForPurchase { get; set; }
    }
}