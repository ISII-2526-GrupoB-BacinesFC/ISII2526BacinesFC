using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.UC_Purchase
{
    public class SelectDevicesPurchase_PO : PageObject
    {
        private By inputFiltroNombre = By.CssSelector("input[placeholder*='Nombre']");
        private By inputFiltroColor = By.CssSelector("input[placeholder*='Color']");
        private By buttonSearch = By.XPath("//button[contains(., 'Buscar')]");
        private By cardDispositivo = By.CssSelector(".col .card");
        private By buttonVaciar = By.XPath("//button[contains(., 'Vaciar')]");
        private By btnTramitar = By.XPath("//button[contains(., 'Tramitar Pedido')]");
        private By alertMessage = By.CssSelector(".alert");


        private IWebElement _rentButton() => _driver.FindElement(btnTramitar);
        public SelectDevicesPurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void SearchDevices(string nombre, string color)
        {
            // Esperar y borrar nombre
            WaitForBeingClickable(inputFiltroNombre);
            _driver.FindElement(inputFiltroNombre).Clear();
            if (!string.IsNullOrEmpty(nombre))
            {
                _driver.FindElement(inputFiltroNombre).SendKeys(nombre);
            }

            // Esperar y borrar color
            _driver.FindElement(inputFiltroColor).Clear();
            if (!string.IsNullOrEmpty(color))
            {
                _driver.FindElement(inputFiltroColor).SendKeys(color);
            }


            _driver.FindElement(buttonSearch).Click();


            Thread.Sleep(1000);
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

