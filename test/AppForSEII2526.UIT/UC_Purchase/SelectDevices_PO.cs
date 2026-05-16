using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.UC_Purchase
{
    public class SelectDevices_PO : PageObject
    {
        // 1. SELECTORES INSENSIBLES A MAYÚSCULAS (Con la 'i' mágica)
        // Busca el primer y segundo cuadro de texto que encuentre en la zona de filtros
        // Buscamos el primer y segundo input que existan en la zona principal de la página, sin importar su tipo
        // Selectores idénticos al HTML de tu pantalla de inspección
        private By inputFiltroNombre = By.CssSelector(".card-body input[placeholder^='Nombre']");
        private By inputFiltroColor = By.CssSelector(".card-body input[placeholder^='Color']");
        private By buttonSearch = By.XPath("//div[contains(@class, 'card-body')]//button[contains(text(), 'Buscar')]");

        private By cardDispositivo = By.CssSelector(".col .card");
        private By buttonVaciar = By.XPath("//button[contains(text(), 'Vaciar')]");
        private By btnTramitar = By.XPath("//button[contains(text(), 'Tramitar Pedido')]");
        private By alertMessage = By.CssSelector(".alert");
        private IWebElement _rentButton() => _driver.FindElement(btnTramitar);

        public SelectDevices_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        // 2. MÉTODO DE BÚSQUEDA CON ESPERAS SEGURAS
        public void SearchDevices(string nombre, string color)
        {
            // 1. Espera de cortesía para que la página se asiente
            Thread.Sleep(1500);
            var js = (IJavaScriptExecutor)_driver;

            try
            {
                // 2. Encontrar los inputs de la sección de filtros
                var inputs = _driver.FindElements(By.CssSelector(".card-body .form-control"));

                if (inputs.Count >= 2)
                {
                    // Forzamos valor y disparamos 'input' y 'change' para despertar a Blazor
                    if (!string.IsNullOrEmpty(nombre))
                    {
                        js.ExecuteScript(
                            "arguments[0].value = arguments[1]; " +
                            "arguments[0].dispatchEvent(new Event('input', { bubbles: true })); " +
                            "arguments[0].dispatchEvent(new Event('change', { bubbles: true }));",
                            inputs[0], nombre);
                    }

                    if (!string.IsNullOrEmpty(color))
                    {
                        js.ExecuteScript(
                            "arguments[0].value = arguments[1]; " +
                            "arguments[0].dispatchEvent(new Event('input', { bubbles: true })); " +
                            "arguments[0].dispatchEvent(new Event('change', { bubbles: true }));",
                            inputs[1], color);
                    }
                }

                // 3. Dejar que Blazor procese los cambios en su modelo C#
                Thread.Sleep(500);

                // 4. Clonamos el botón de Buscar apuntando directo a la tarjeta gris (.bg-light)
                // Así evitamos que Selenium se confunda con los botones de "Añadir al Carrito"
                var botonBuscar = _driver.FindElement(By.CssSelector(".bg-light button"));

                // 5. Hacemos el click fulminante por JavaScript
                js.ExecuteScript("arguments[0].click();", botonBuscar);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"[ERROR] Error en la búsqueda: {ex.Message}");
                throw;
            }

            // 6. Pausa para ver cómo desaparecen los móviles tras el filtro
            Thread.Sleep(2000);
        }


        public bool CheckListOfDevices(List<string[]> expectedData)
        {
            var cards = _driver.FindElements(cardDispositivo);


            if (cards.Count == 0 && expectedData.Count > 0) return false;

            foreach (var expected in expectedData)
            {
                bool found = false;
                string expectedNombre = expected[0];
                string expectedMarca = expected[1];
                string expectedPrecio = expected[2];

                foreach (var card in cards)
                {

                    string actualNombre = card.FindElement(By.CssSelector(".card-title")).Text;
                    string actualMarca = card.FindElement(By.CssSelector(".card-subtitle")).Text;
                    string actualPrecio = card.FindElement(By.CssSelector("h3.text-primary")).Text;

                    if (actualNombre.Contains(expectedNombre, StringComparison.OrdinalIgnoreCase) &&
                        actualMarca.Contains(expectedMarca, StringComparison.OrdinalIgnoreCase) &&
                        actualPrecio.Contains(expectedPrecio))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    _output.WriteLine($"No se encontró TARJETA para: {expectedNombre} | {expectedMarca} | {expectedPrecio}");
                    return false;
                }
            }
            return true;
        }


        public void AddDeviceToCart(string nombreDispositivo)
        {

            var xpathButton = $"//h5[contains(@class,'card-title') and contains(text(),'{nombreDispositivo}')]/ancestor::div[contains(@class,'card')]//button[contains(., 'Añadir')]";

            By btnAdd = By.XPath(xpathButton);

            WaitForBeingClickable(btnAdd);
            _driver.FindElement(btnAdd).Click();

            Thread.Sleep(500);
        }


        public void EmptyCart()
        {
            if (!IsProceedToCheckoutHidden())
            {
                WaitForBeingClickable(buttonVaciar);
                _driver.FindElement(buttonVaciar).Click();
            }
        }



        public bool IsProceedToCheckoutHidden()
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            var elementos = _driver.FindElements(btnTramitar);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            if (elementos.Count == 0) return true;


            return !elementos[0].Displayed;
        }

        public void RemoveDeviceFromCart(string nombreDispositivo)
        {

            var xpathBoton = $"//li[contains(., '{nombreDispositivo}')]//button[contains(@class, 'btn-remove')]";

            By btnEliminar = By.XPath(xpathBoton);

            try
            {
                WaitForBeingClickable(btnEliminar);
                _driver.FindElement(btnEliminar).Click();


                Thread.Sleep(1000);
            }
            catch (WebDriverTimeoutException)
            {

                _output.WriteLine($"Error: No se encontró el botón de borrar (clase .btn-remove) para el móvil '{nombreDispositivo}'.");
                throw;
            }
        }


        private By totalPrecio = By.XPath("//div[contains(@class, 'card-footer')]//strong[contains(@class, 'text-primary') or contains(@class, 'h4') or contains(@class, 'h5')]");

        public string GetTotalPrice()
        {
            try
            {

                WaitForBeingVisible(totalPrecio);

                string texto = _driver.FindElement(totalPrecio).Text;



                return texto;
            }
            catch (WebDriverTimeoutException)
            {
                _output.WriteLine("Error: No se encontró el elemento del precio total en el tiempo límite.");
                return "0,00 €";
            }
        }

        public bool CheckTotalPrice(string expectedPrice)
        {
            string actualPrice = GetTotalPrice();
            _output.WriteLine($"Comparando precio total: esperado='{expectedPrice}' vs actual='{actualPrice}'");
            return actualPrice.Contains(expectedPrice);
        }


        public bool CheckMessageErrorNotAvaibleDevices(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);

        }

        public void ProceedToCheckout()
        {
            WaitForBeingClickable(btnTramitar);
            _driver.FindElement(btnTramitar).Click();

        }
    }
}

