using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.API.Models
{
    public class PurchaseItem
    {
        [Key]
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "Descripción (Opcional)")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        // Claves foráneas
        [Required]
        public int DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; }

        [Required]
        public int PurchaseId { get; set; }
        [ForeignKey("PurchaseId")]
        public virtual Purchase Purchase { get; set; }
        public double PriceAtPurchase { get; internal set; }
    }
}