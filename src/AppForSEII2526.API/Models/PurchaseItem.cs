namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId), nameof(PurchaseId))]
    public class PurchaseItem
    {
        public PurchaseItem() { }

        // CAMBIO: deviceId ahora es int
        public PurchaseItem(int deviceId, int purchaseId, decimal price, int quantity)
        {
            DeviceId = deviceId;
            PurchaseId = purchaseId;
            Price = price;
            Quantity = quantity;
        }

        [StringLength(150, ErrorMessage = "La descripción no puede ser superior a 150 caracteres.")]
        public string? Description { get; set; }

        // CAMBIO: Debe ser int para que coincida con Device.Id
        public int DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public int PurchaseId { get; set; }

        public virtual Purchase Purchase { get; set; }

        [Precision(10, 2)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must provide a quantity higher than 1")]
        public int Quantity { get; set; }
    }
}