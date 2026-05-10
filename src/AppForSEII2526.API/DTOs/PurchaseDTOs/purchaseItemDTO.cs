namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class purchaseItemDTO
    {
        public purchaseItemDTO() 
        { 
        }
        public purchaseItemDTO(string brand, string model, string color, double price, int quantity, string? description = null)
        {
            Brand = brand;
            Model = model;
            Color = color;
            Price = price;
            Quantity = quantity;
            Description = description;

        }



        [StringLength(50, ErrorMessage = "La marca no puede ser mayor de 50 caracteres")]
        public string Brand { get; set; }
        public string Model { get; set; }

        [StringLength(20, ErrorMessage = "El color no puede ser mayor de 20 caracteres ni menor que 1 ", MinimumLength = 1)]
        public string Color { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, float.MaxValue, ErrorMessage = "El precio mínimo es de 0,5 ")]
        [Display(Name = "Precio para compra")]
        [Precision(10, 2)]
        public double Price { get; set; }

        [StringLength(150, ErrorMessage = "La descripcion no puede ser mayor de 150 caracteres")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad mínima es 1")]

        public int Quantity { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is purchaseItemDTO dTO &&
                   Brand == dTO.Brand &&
                   Model == dTO.Model &&
                   Color == dTO.Color &&
                   Price == dTO.Price &&
                   Quantity == dTO.Quantity &&
                   Description == dTO.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Brand, Model, Color, Price, Quantity, Description);
        }
    }
}
