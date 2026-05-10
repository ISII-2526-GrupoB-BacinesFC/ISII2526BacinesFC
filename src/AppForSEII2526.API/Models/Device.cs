namespace AppForSEII2526.API.Models
{
    public class Device
    {
        public Device() { }

        public Device(Model model, string brand, string color, string name, decimal priceForPurchase, int quantityForPurchase, int year)
        {
            Model = model;
            Brand = brand;
            Color = color;
            Name = name;
            PriceForPurchase = priceForPurchase;
            QuantityForPurchase = quantityForPurchase;
            Year = year;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public Model Model { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; }

        [Required]
        [StringLength(30)]
        public string Color { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Precision(10, 2)] // Para que en la BD se guarde bien el dinero
        public decimal PriceForPurchase { get; set; }

        [Required]
        public int QuantityForPurchase { get; set; }

        [Required]
        public int Year { get; set; }
    }
}