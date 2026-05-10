using System.Collections.Generic;

namespace AppForSEII2526.API.DTOs
{
    public class PurchaseCreateDTO
    {
        public string CustomerUserName { get; set; }
        public string CustomerUserSurname { get; set; }
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; } // Lo recibimos como string y lo convertimos en el Controller
        public List<PurchaseItemCreateDTO> Items { get; set; }
    }
}