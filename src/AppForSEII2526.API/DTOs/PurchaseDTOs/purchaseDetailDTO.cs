using AppForSEII2526.API.Models;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class purchaseDetailDTO
    {

        public purchaseDetailDTO(string nameUsuario, string surenameUsuario, string deliveryAddress, DateTime purchaseDateFrom, double totalPrice, int totalQuantity, IList<purchaseItemDTO> purchaseItems)
        {
            name = nameUsuario;
            surename = surenameUsuario;
            DeliveryAddress = deliveryAddress;
            PurchaseDateFrom = purchaseDateFrom;
            TotalPrice = totalPrice;
            TotalQuantity = totalQuantity;
            PurchaseItems = purchaseItems;
        }

        public int Id { get; set; }

        //NOMBRE USUARIO

        [StringLength(40, ErrorMessage = "El nombre del usuario no puede ser superior a 40 carecteres")]
        public string name { get; set; }

        //APELLIDOS 
        [StringLength(40, ErrorMessage = "Los apellidos del usuario no pueden ser superiores a 40 carecteres")]
        public string surename { get; set; }
        //DIRECCION
        [Required]
        [StringLength(100, ErrorMessage = "La direccion no puede ser superior a 100 carecteres")]
        public string DeliveryAddress { get; set; }

        //FECHA DE COMPRA
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [Required, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de compra")]
        public DateTime PurchaseDateFrom { get; set; }

        //PRECIO TOTAL
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, double.MaxValue, ErrorMessage = "Precio mínimo es O,5")]
        [Display(Name = "Precio Total")]
        public double TotalPrice { get; set; }

        //CANTIDAD TOTAL
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidd minima es 1")]
        [Display(Name = "Cantidad Total")]
        public int TotalQuantity { get; set; }

        public IList<purchaseItemDTO> PurchaseItems { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not purchaseDetailDTO dTO)
                return false;

            // 🔸 Tolerancia de 1 segundo en la comparación de fechas
            bool fechasCasiIguales = Math.Abs((PurchaseDateFrom - dTO.PurchaseDateFrom).TotalSeconds) < 1;

            return
                name == dTO.name &&
                surename == dTO.surename &&
                DeliveryAddress == dTO.DeliveryAddress &&
                fechasCasiIguales &&
                Math.Abs(TotalPrice - dTO.TotalPrice) < 0.01 &&
                TotalQuantity == dTO.TotalQuantity &&
                PurchaseItems.SequenceEqual(dTO.PurchaseItems);
        }
    }
}