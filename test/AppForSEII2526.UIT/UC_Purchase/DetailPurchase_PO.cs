using AppForSEII2526.UIT.Shared;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.UC_Purchase
{
    public class DetailPurchase_PO : PageObject
    {
        private By labelNameSurname = By.Id("NameSurname");
        private By labelAddress = By.Id("DeliveryAddress");
        private By fechaCompra = By.Id("FechaCompra");
        private By labelTotalPrice = By.Id("TotalPrice");

        // CORREGIDO: Buscamos directamente por la etiqueta HTML 'table' sin depender de IDs viejos
        private By tableMovies = By.TagName("table");

        public DetailPurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool VerificarDetallesCabecera(string nombreCompleto, string direccion, string fecha, string precioTotal)
        {
            try
            {
                // Esperamos a que cargue la página de detalle
                WaitForBeingVisible(labelNameSurname);

                string actualName = _driver.FindElement(labelNameSurname).Text;
                string actualAddress = _driver.FindElement(labelAddress).Text;
                string actualFecha = _driver.FindElement(fechaCompra).Text;
                string actualPrice = _driver.FindElement(labelTotalPrice).Text;

                _output.WriteLine($"Detalle encontrado -> Nombre: {actualName}, Direccion: {actualAddress}, Precio: {actualPrice}, Fecha: {actualFecha}");

                // Validamos
                bool checkName = actualName.Contains(nombreCompleto);
                bool checkAddr = actualAddress.Contains(direccion);
                bool checkPrice = actualPrice.Contains(precioTotal);
                bool checkFecha = actualFecha.Contains(fecha);

                _output.WriteLine($"DATOS EN LA WEB: {actualName} | {actualAddress} | {actualFecha} | {actualPrice}");
                _output.WriteLine($"DATOS ESPERADOS: {nombreCompleto} | {direccion} | {fecha} | {precioTotal}");

                return checkName && checkAddr && checkFecha && checkPrice;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error verificando cabecera: {ex.Message}");
                return false;
            }
        }

        public bool CheckListOfDispositivos(List<string[]> expectedData)
        {
            // 1. Esperamos a que la tabla física aparezca en pantalla
            WaitForBeingVisible(tableMovies);

            // Un pequeño respiro de seguridad para que Blazor termine de renderizar las celdas internas
            Thread.Sleep(500);

            CultureInfo culturaES = new CultureInfo("es-ES");

            // CORREGIDO: Buscamos las filas de la tabla de forma genérica
            var filas = _driver.FindElements(By.CssSelector("table tbody tr"));

            if (filas.Count == 0 && expectedData.Count > 0) return false;

            foreach (var expected in expectedData)
            {
                bool found = false;

                string expectedNombre = expected[0];
                string expectedMarca = expected[1];
                string expectedColor = expected[2];
                string expectedPrecio = expected[3];
                string expectedCantidad = expected[4];
                string expectedDescripcion = expected[5];

                foreach (var fila in filas)
                {
                    var columnas = fila.FindElements(By.TagName("td"));

                    // Si la fila no tiene columnas suficientes, saltamos
                    if (columnas.Count < 6) continue;

                    string actualNombre = columnas[0].Text.Trim();
                    string actualMarca = columnas[1].Text.Trim();
                    string actualColor = columnas[2].Text.Trim();

                    // Limpiamos el símbolo del euro para comparar solo el número ("1.399,99")
                    string actualPrecio = columnas[3].Text.Replace("€", "").Trim();
                    string actualCantidad = columnas[4].Text.Trim();
                    string actualDescripcion = columnas[5].Text.Trim();

                    _output.WriteLine($"DATOS EN LA WEB: {actualNombre} | {actualMarca} | {actualColor} | {actualPrecio} | {actualCantidad} | {actualDescripcion}");
                    _output.WriteLine($"DATOS ESPERADOS: {expectedNombre} | {expectedMarca} | {expectedColor} | {expectedPrecio} | {actualCantidad} | {expectedDescripcion}");

                    if (actualNombre.Contains(expectedNombre, StringComparison.OrdinalIgnoreCase) &&
                        actualMarca.Contains(expectedMarca, StringComparison.OrdinalIgnoreCase) &&
                        actualColor.Contains(expectedColor, StringComparison.OrdinalIgnoreCase) &&
                        actualPrecio.Contains(expectedPrecio) &&
                        actualCantidad.Contains(expectedCantidad) &&
                        actualDescripcion.Contains(expectedDescripcion, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    _output.WriteLine($"No se encontró FILA para: {expectedNombre} | {expectedMarca} | {expectedColor} | {expectedPrecio} | {expectedCantidad} | {expectedDescripcion}");
                    return false;
                }
            }

            return true;
        }
    }
}