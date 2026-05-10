namespace AppForSEII2526.API.Models
{
    public enum PaymentMethodTypes
    {
        CreditCard,
        PayPal,
        Cash
    }
    public class Purchase
    {

        [Key]
        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        //TOTAL PRICE
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, double.MaxValue, ErrorMessage = "Minimum price is 0.5")]
        [Display(Name = "Total Price")]
        public double TotalPrice { get; set; }

        //TOTAL QUANTITY
        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum quantity is 1")]
        [Display(Name = "Total Quantity")]
        public int TotalQuantity { get; set; }




        public IList<PurchaseItem> PurchaseItems { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Column("MetodoDePago")]
        public PaymentMethodTypes PaymentMethodTypes { get; set; }
        private Purchase()
        {

        }

        public Purchase(PaymentMethodTypes paymentMethodTypes, DateTime purchaseDate, IList<PurchaseItem> purchaseItems, ApplicationUser applicationUser)
        {
            PaymentMethodTypes = paymentMethodTypes;
            PurchaseDate = purchaseDate;
            PurchaseItems = purchaseItems;
            ApplicationUser = applicationUser;
        }

        public override bool Equals(object? obj)
        {
            return obj is Purchase purchase &&
                   Id == purchase.Id &&
                   PurchaseDate == purchase.PurchaseDate &&
                   TotalPrice == purchase.TotalPrice &&
                   TotalQuantity == purchase.TotalQuantity &&
                   EqualityComparer<IList<PurchaseItem>>.Default.Equals(PurchaseItems, purchase.PurchaseItems) &&
                   EqualityComparer<ApplicationUser>.Default.Equals(ApplicationUser, purchase.ApplicationUser) &&
                   PaymentMethodTypes == purchase.PaymentMethodTypes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, PurchaseDate, TotalPrice, TotalQuantity, PurchaseItems, ApplicationUser, PaymentMethodTypes);
        }
    }
}