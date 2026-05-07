using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.API.Models
{
    public enum PaymentMethod
    {
        CreditCard,
        PayPal
    }

    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nombre del Cliente")]
        public string CustomerUserName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Apellidos del Cliente")]
        public string CustomerUserSurname { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Dirección de Entrega")]
        public string DeliveryAddress { get; set; }

        [Required]
        [Display(Name = "Método de Pago")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [Display(Name = "Fecha de Compra")]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Display(Name = "Precio Total")]
        public double TotalPrice { get; set; }

        [Required]
        [Display(Name = "Cantidad Total")]
        public int TotalQuantity { get; set; }

        public virtual IList<PurchaseItem> PurchaseItems { get; set; }
        public string CustomerId { get; internal set; }
    }
}