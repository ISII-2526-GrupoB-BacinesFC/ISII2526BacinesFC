namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class purchaseForCreateDTO
    {
        public purchaseForCreateDTO(string customerUserName, string customerNameSurname, string deliveryAddress, PaymentMethodTypes paymentMethod, int quantity, IList<purchaseItemDTO> purchaseItems)
        {
            CustomerUserName = customerUserName ?? throw new ArgumentNullException(nameof(customerUserName));
            CustomerNameSurname = customerNameSurname ?? throw new ArgumentNullException(nameof(customerNameSurname));
            DeliveryAddress = deliveryAddress ?? throw new ArgumentNullException(nameof(deliveryAddress));
            PaymentMethod = paymentMethod;
            Quantity = quantity;
            PurchaseItems = purchaseItems ?? throw new ArgumentNullException(nameof(purchaseItems));
        }

        public purchaseForCreateDTO()
        {
            PurchaseItems = new List<purchaseItemDTO>();
        }


        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Delivery Address")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Delivery address must have at least 4 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your address for delivery")]
        public string DeliveryAddress { get; set; }

        [Required]
        public string CustomerUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your Name and Surname")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Name and Surname must have at least 4 characters")]
        public string CustomerNameSurname { get; set; }

        public IList<purchaseItemDTO> PurchaseItems { get; set; }
        [Required]
        public PaymentMethodTypes PaymentMethod { get; set; }

        public int Quantity { get; set; }

        public DateTime DateTime
        {
            get { return DateTime.Now; }
        }

        public override bool Equals(object? obj)
        {
            return obj is purchaseForCreateDTO dto &&
                   CustomerUserName == dto.CustomerUserName &&
                   CustomerNameSurname == dto.CustomerNameSurname &&
                   DeliveryAddress == dto.DeliveryAddress &&
                   PaymentMethod == dto.PaymentMethod &&
                   Quantity == dto.Quantity &&
                   PurchaseItems.SequenceEqual(dto.PurchaseItems);
        }
    }
}
