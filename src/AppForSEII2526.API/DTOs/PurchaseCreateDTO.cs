using System.Collections.Generic;

namespace AppForSEII2526.API.DTOs
{
    public class PurchaseCreateDTO
    {
        public string CustomerId { get; set; }
        public List<PurchaseItemCreateDTO> Items { get; set; }
    }
}