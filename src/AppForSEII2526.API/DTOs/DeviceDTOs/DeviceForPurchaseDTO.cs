namespace AppForSEII2526.API.DTOs.DeviceDTOs
{
    public class DeviceForPurchaseDTO
    {
        public DeviceForPurchaseDTO(int id, string nameDevice, string brand, Model model, string color, double priceForPurchase)
        {
            Id = id;
            nameDevice = nameDevice;
            Brand = brand;
            Model = model;
            Color = color;
            priceForPurchase = priceForPurchase;
        }

        public int Id { get; set; }

        //NOMBRE DISPOSITIVO
        [Required]
        [StringLength(50, ErrorMessage = "El nombre del dispositivo no puede ser mayor de 50 caracteres")]
        public string nameDevice { get; set; }

        //MARCA
        [Required]
        [StringLength(50, ErrorMessage = "La marca no puede ser mayor de 50 caracteres")]
        public string Brand { get; set; }

        //MODELO
        [Required]
        public Model Model { get; set; }

        //COLOR
        [Required]
        [StringLength(20, ErrorMessage = "El color no puede ser mayor de 20 caracteres")]
        public string Color { get; set; }
        //PRECIO COMPRA
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, float.MaxValue, ErrorMessage = "El precio mínimo es de 0,5 ")]
        [Display(Name = "Precio para compra")]
        [Precision(10, 2)]
        public double priceForPurchase { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DeviceForPurchaseDTO dTO &&
                   Id == dTO.Id &&
                   nameDevice == dTO.nameDevice &&
                   Brand == dTO.Brand &&
                   EqualityComparer<Model>.Default.Equals(Model, dTO.Model) &&
                   Color == dTO.Color &&
                   priceForPurchase == dTO.priceForPurchase;
        }
    }
}
