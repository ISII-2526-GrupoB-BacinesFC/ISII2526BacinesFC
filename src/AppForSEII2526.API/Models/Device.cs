using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AppForSEII2526.API.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Marca")]
        public string Brand { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Color")]
        public string Color { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Precio de compra")]
        public double priceForPurchase { get; set; }

        [Display(Name = "Precio de alquiler")]
        public double priceForRent { get; set; }

        [Required]
        [Display(Name = "Cantidad para compra")]
        public int quantityForPurchase { get; set; }

        [Display(Name = "Cantidad para alquiler")]
        public int quantityForRent { get; set; }

        [Required]
        [Display(Name = "Año")]
        public int Year { get; set; }

        // Relaciones
        public virtual Model Model { get; set; }
        public virtual IList<PurchaseItem> PurchaseItems { get; set; }
    }
}