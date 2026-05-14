using AppForSEII2526.Web.API;
namespace AppForSEII2526.Web
{
    public class PurchasesStateContainer
    {

        public PurchaseForCreateDTO Purchase { get; private set; }

        public PurchasesStateContainer()
        {
            ResetCompra();
        }
        public event Action? OnStateChange;
        private void NotifyStateChanged() => OnStateChange?.Invoke();

        public void ResetCompra()
        {

            Purchase = new PurchaseForCreateDTO
            {
                CustomerUserName = string.Empty,
                CustomerNameSurname = string.Empty,
                DeliveryAddress = string.Empty,
                PaymentMethod = PaymentMethodTypes.CreditCard,
                Quantity = 0,
                PurchaseItems = new List<PurchaseItemDTO>()
            };
            NotifyStateChanged();
        }

        public decimal TotalPrice
        {
            get
            {
                if (Purchase.PurchaseItems == null) return 0;
                return (decimal)Purchase.PurchaseItems.Sum(item => item.Price * item.Quantity);
            }
        }

        public void AñadirDispositivo(DeviceForPurchaseDTO dispositivo)
{
    // Buscamos si ya existe (usando los mismos criterios que usa tu DTO)
    var itemExistente = Purchase.PurchaseItems.FirstOrDefault(ic =>
        ic.Brand == dispositivo.Brand &&
        ic.Model == (dispositivo.Model?.Name ?? "Sin Modelo") &&
        ic.Color == dispositivo.Color
    );

    if (itemExistente != null)
    {
        itemExistente.Quantity++;
    }
    else
    {
        // Creamos el ítem asegurándonos de que NADA sea nulo
        var nuevoItem = new PurchaseItemDTO // Usa el nombre exacto de tu clase
        {
            Brand = dispositivo.Brand ?? "Desconocida",
            Model = dispositivo.Model?.Name ?? dispositivo.NameDevice ?? "Genérico",
            Color = dispositivo.Color ?? "N/A",
            Price = dispositivo.PriceForPurchase,
            Quantity = 1,
            Description = "Compra realizada desde la web"
        };

        Purchase.PurchaseItems.Add(nuevoItem);
    }

    ActualizarTotales();
    NotifyStateChanged();
}

        public void EliminarCarrito()
        {
            Purchase.PurchaseItems.Clear();
            ActualizarTotales();
            NotifyStateChanged();
        }

        public void QuitarItemParaComprar(PurchaseItemDTO item)
        {
            if (Purchase.PurchaseItems.Contains(item))
            {
                Purchase.PurchaseItems.Remove(item);
                ActualizarTotales();
                NotifyStateChanged();
            }
        }

        private void ActualizarTotales()
        {
            Purchase.Quantity = Purchase.PurchaseItems.Sum(i => i.Quantity);
        }
    }
}