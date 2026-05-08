using System.Collections.Generic;

namespace AppForSEII2526.API.DTOs
{
    public class PurchaseCreateDTO
    {
        public string CustomerUserName { get; set; }
        public string CustomerUserSurname { get; set; }
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; }
        public List<PurchaseItemCreateDTO> Items { get; set; }
    }
}