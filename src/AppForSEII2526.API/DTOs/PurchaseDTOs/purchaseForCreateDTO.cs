namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class purchaseForCreateDTO
    {
        public purchaseForCreateDTO(string customerUserName, string customerNameSurname, string deliveryAddress, PaymentMethodTypes paymentMethod, DateTime purchaseDateFrom, DateTime purchaseDateTo, IList<purchaseItemDTO> purchaseItems)
        {
            CustomerUserName = customerUserName ?? throw new ArgumentNullException(nameof(customerUserName));
            CustomerNameSurname = customerNameSurname ?? throw new ArgumentNullException(nameof(customerNameSurname));
            DeliveryAddress = deliveryAddress ?? throw new ArgumentNullException(nameof(deliveryAddress));
            PaymentMethod = paymentMethod;
            PurchaseDateFrom = purchaseDateFrom;
            PurchaseDateTo = purchaseDateTo;
            PurchaseItems = purchaseItems ?? throw new ArgumentNullException(nameof(purchaseItems));
        }

        public purchaseForCreateDTO()
        {
            PurchaseItems = new List<purchaseItemDTO>();
        }

        public DateTime PurchaseDateFrom { get; set; }

        public DateTime PurchaseDateTo { get; set; }


        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Delivery Address")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Delivery address must have at least 10 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your address for delivery")]
        public string DeliveryAddress { get; set; }

        [EmailAddress]
        [Required]
        public string CustomerUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your Name and Surname")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Name and Surname must have at least 10 characters")]
        public string CustomerNameSurname { get; set; }

        public IList<purchaseItemDTO> PurchaseItems { get; set; }
        [Required]
        public PaymentMethodTypes PaymentMethod { get; set; }

        private int NumberOfDays
        {
            get
            {
                return (PurchaseDateTo - PurchaseDateFrom).Days;
            }
        }

        [Display(Name = "Total Price")]
        [JsonPropertyName("TotalPrice")]
        [Precision(10, 2)]
        public decimal TotalPrice
        {
            get
            {
                return PurchaseItems.Sum(ri => ri.Price * NumberOfDays);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is purchaseForCreateDTO dTO &&
                   CompareDate(PurchaseDateFrom, dTO.PurchaseDateFrom) &&
                   CompareDate(PurchaseDateTo, dTO.PurchaseDateTo) &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   CustomerUserName == dTO.CustomerUserName &&
                   CustomerNameSurname == dTO.CustomerNameSurname &&
                   PurchaseItems.SequenceEqual(dTO.PurchaseItems) &&
                   PaymentMethod == dTO.PaymentMethod &&
                   TotalPrice == dTO.TotalPrice;
        }
    }
}
