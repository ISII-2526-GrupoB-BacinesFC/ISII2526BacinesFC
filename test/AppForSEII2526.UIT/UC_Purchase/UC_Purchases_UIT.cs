using AppForSEII2526.UIT.Shared;
using AppForSEII2526.UIT.UC_Purchase;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.UC_Purchase
{
    public class UC_Purchases_UIT : UC_UIT
    {
        private SelectDevices_PO _selectPO;

        public UC_Purchases_UIT(ITestOutputHelper output) : base(output)
        {
            _selectPO = new SelectDevices_PO(_driver, _output);
        }

        private void InitialStepsForCompra()
        {
            _driver.Navigate().GoToUrl(_URI + "Purchases/SelectDevices");
        }

        // ==========================================
        // PRUEBAS DEL SELECCIONAR DISPOSITIVOS
        // ==========================================

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_2_No_hay_Dispositivos()
        {
            InitialStepsForCompra();

            string colorInexistente = "sajfbuf";
            string mensajeEsperado = "No se han encontrado dispositivos con esos filtros.";

            _selectPO.SearchDevices("", colorInexistente);

            Assert.True(_selectPO.CheckMessageErrorNotAvaibleDevices(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");
        }

        [Theory]
        [Trait("LevelTesting", "Funcional Testing")]
        [InlineData("Galaxy", "", "Galaxy S24 Ultra 512GB", "Samsung", "1.399,99 €")]
        [InlineData("", "Blanco", "iPhone 15 Pro Max 256GB", "Apple", "1.499,00 €")]
        public void UC1_3Y4_Compra_FiltrarDispositivos(string filtroNombre, string filtroColor, string nombreEsperado, string marcaEsperada, string precioEsperado)
        {
            InitialStepsForCompra();

            var expectedDispositivos = new List<string[]>
            {
                new string[] { nombreEsperado, marcaEsperada, precioEsperado }
            };

            _selectPO.SearchDevices(filtroNombre, filtroColor);

            Assert.True(_selectPO.CheckListOfDevices(expectedDispositivos),
                $"Error: No se encontró la tarjeta con: {nombreEsperado} | {marcaEsperada}");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_5_GestionCarrito_RecalculoPrecios()
        {
            InitialStepsForCompra();

            string movil1 = "Galaxy S24 Ultra 512GB";
            string movil2 = "iPhone 15 Pro Max 256GB";
            string precioEsperadoFinal = "1.499,00 €";

            _selectPO.SearchDevices("Galaxy", "");
            _selectPO.AddDeviceToCart(movil1); // Usa el nombre largo para la tarjeta

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart(movil2);

            // CORREGIDO: Le pasamos solo "Galaxy S24 Ultra" o "Galaxy" para que el Contains del carrito lo cace al vuelo
            _selectPO.RemoveDeviceFromCart("Galaxy S24 Ultra");

            Assert.True(_selectPO.CheckTotalPrice(precioEsperadoFinal));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC1_6Compra_Carrito_Vacio_Oculta_Tramitar()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.EmptyCart();

            Assert.True(_selectPO.IsProceedToCheckoutHidden(), "El botón 'Tramitar Pedido' debería ocultarse tras vaciar el carrito.");
        }

        // ==========================================
        // PRUEBAS DEL FORMULARIO CREAR COMPRA
        // ==========================================

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_7_Nombre_Vacio()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeError = "Por favor, introduce tu Nombre.";

            crearCompraPO.EscribirNombre("");
            crearCompraPO.EscribirApellidos("Pérez García");
            crearCompraPO.EscribirDireccion("Avenida Libertad 45, Barcelona");
            crearCompraPO.SeleccionarPago("Cash");
            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_8_Apellidos_Vacio()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeError = "Por favor, introduce tus Apellidos.";

            crearCompraPO.EscribirNombre("elena@uclm.es");
            crearCompraPO.EscribirApellidos("");
            crearCompraPO.EscribirDireccion("Calle Mayor 123, Madrid");
            crearCompraPO.SeleccionarPago("Cash");
            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_9_Direccion_Vacio()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeError = "Es obligatorio introducir una Dirección de entrega.";

            crearCompraPO.EscribirNombre("elena@uclm.es");
            crearCompraPO.EscribirApellidos("Pérez García");
            crearCompraPO.EscribirDireccion("");
            crearCompraPO.SeleccionarPago("Cash");
            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_10_Usuario_No_Existe()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            // CORREGIDO: Al pasar la validación frontend del email, la API ya devuelve su mensaje real controlado
            string mensajeEsperado = "El usuario no existe.";
            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            crearCompraPO.EscribirNombre("usuario_inexistente_pero_formato_email@test.com");
            crearCompraPO.EscribirApellidos("Navarro Martínez");
            crearCompraPO.EscribirDireccion("Avda. España 2, Albacete");
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_11_Dispositivo_sin_stock()
        {
            InitialStepsForCompra();

            string dispositivo = "iPhone 15 Pro Max 256GB";
            _selectPO.SearchDevices("iPhone", "");

            for (int i = 0; i < 15; i++)
            {
                _selectPO.AddDeviceToCart(dispositivo);
            }

            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeEsperado = "No hay stock suficiente para el dispositivo seleccionado.";

            // ACT
            crearCompraPO.EscribirNombre("elena@uclm.es");
            crearCompraPO.EscribirApellidos("Navarro Martínez"); // CORREGIDO: Ponemos su apellido real de la BD
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_12_Volver_Desde_CrearCompra_Mantiene_Carrito()
        {
            InitialStepsForCompra();

            string movilPrueba = "Galaxy S24 Ultra 512GB";
            string precioAntesDeIrse = "1.399,99 €";

            _selectPO.SearchDevices("Galaxy", "");
            _selectPO.AddDeviceToCart(movilPrueba);
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            crearCompraPO.ClickVolver();

            Assert.True(_selectPO.CheckTotalPrice(precioAntesDeIrse));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_13_Nombre_Excede_Longitud()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            string nombreLargo = new string('a', 51) + "@uclm.es";

            // CORREGIDO: Mensaje explícito e inteligente del Frontend de Blazor
            string mensajeError = "El Nombre es demasiado largo (máximo 50 caracteres).";

            crearCompraPO.EscribirNombre(nombreLargo);
            crearCompraPO.EscribirApellidos("Gómez Fernández");
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_14_Apellidos_Excede_Longitud()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string apellidosLargos = new string('a', 71);

            // CORREGIDO: Mensaje explícito e inteligente del Frontend de Blazor
            string mensajeError = "Los Apellidos son demasiado largos (máximo 70 caracteres).";

            crearCompraPO.EscribirNombre("elena@uclm.es");
            crearCompraPO.EscribirApellidos(apellidosLargos);
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_15_Direccion_Excede_Longitud()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string direccionLarga = new string('a', 101);

            // CORREGIDO: Mensaje explícito e inteligente del Frontend de Blazor
            string mensajeError = "La Dirección es demasiado larga (máximo 100 caracteres).";

            crearCompraPO.EscribirNombre("elena@uclm.es");
            crearCompraPO.EscribirApellidos("Gómez Fernández");
            crearCompraPO.EscribirDireccion(direccionLarga);
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_1_Flujo_Basico()
        {
            InitialStepsForCompra();

            string cardMovil1 = "Galaxy S24 Ultra 512GB";
            string cardMovil2 = "iPhone 15 Pro Max 256GB";

            string nombreRecibo1 = "Galaxy S24 Ultra";
            string marcaEsperada = "Samsung";
            string colorEsperado = "Negro";
            string precioEsperado = "1.399,99";
            string cantidadEsperada = "1";
            string descripcionEsperada = "Compra realizada desde la web";

            string nombreRecibo2 = "iPhone 15 Pro Max";
            string marcaEsperada2 = "Apple";
            string colorEsperado2 = "Blanco";
            string precioEsperado2 = "1.499,00";
            string cantidadEsperada2 = "1";
            string descripcionEsperada2 = "Compra realizada desde la web";

            string nombreUser = "elena@uclm.es";
            string apellidosUser = "Navarro Martínez";
            string direccionUser = "Avda. España 2, Albacete";

            _selectPO.SearchDevices("Galaxy", "");
            _selectPO.AddDeviceToCart(cardMovil1);

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart(cardMovil2);
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            var _detallePO = new DetailPurchase_PO(_driver, _output);

            crearCompraPO.EscribirNombre(nombreUser);
            crearCompraPO.EscribirApellidos(apellidosUser);
            crearCompraPO.EscribirDireccion(direccionUser);
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            string precioTotalEsperado = "2.898,99 €";
            string fechaEsperada = DateTime.Now.ToString("dd/MM/yyyy");

            Assert.True(_detallePO.VerificarDetallesCabecera(
                $"{nombreUser} {apellidosUser}",
                direccionUser,
                fechaEsperada,
                precioTotalEsperado),
                "Los datos de la cabecera del detalle (Nombre, Dirección, Pago o Precio) son incorrectos.");

            List<string[]> dispositivosEsperados = new List<string[]>
            {
                new string[] { nombreRecibo1, marcaEsperada, colorEsperado, precioEsperado, cantidadEsperada, descripcionEsperada },
                new string[] { nombreRecibo2, marcaEsperada2, colorEsperado2, precioEsperado2, cantidadEsperada2, descripcionEsperada2 }
            };

            Assert.True(
                _detallePO.CheckListOfDispositivos(dispositivosEsperados),
                $"Los dispositivos comprados no coinciden o no aparecen en la tabla de detalles del recibo."
            );
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_16_Informacion_Carrito()
        {
            InitialStepsForCompra();

            string cardMovil1 = "Galaxy S24 Ultra 512GB";
            string cardMovil2 = "iPhone 15 Pro Max 256GB";

            // PROFESIONAL: El valor esperado coincide al milímetro con "@item.Brand @item.Model" de la web
            string nombreEsperado = "Samsung Galaxy S24 Ultra";
            string colorEsperado = "Negro";
            string precioEsperado = "1.399,99 €";

            string nombreEsperado2 = "Apple iPhone 15 Pro Max";
            string colorEsperado2 = "Blanco";
            string precioEsperado2 = "1.499,00 €";

            string nombreUser = "elena@uclm.es";
            string apellidosUser = "Navarro Martínez";
            string direccionUser = "Avda. España 2, Albacete";

            // Acciones de búsqueda (con la limpieza de filtros que pusimos)
            _selectPO.SearchDevices("Galaxy", "Negro");
            _selectPO.AddDeviceToCart(cardMovil1);

            _selectPO.SearchDevices("iPhone", "Blanco");
            _selectPO.AddDeviceToCart(cardMovil2);
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            crearCompraPO.EscribirNombre(nombreUser);
            crearCompraPO.EscribirApellidos(apellidosUser);
            crearCompraPO.EscribirDireccion(direccionUser);
            crearCompraPO.SeleccionarPago("Cash");

            List<string[]> dispositivosEsperados = new List<string[]>
    {
        new string[] { nombreEsperado, colorEsperado, precioEsperado },
        new string[] { nombreEsperado2, colorEsperado2, precioEsperado2 }
    };

            Assert.True(
                crearCompraPO.CheckListOfDispositivosEnCarrito(dispositivosEsperados),
                "Los dispositivos o sus precios no se muestran correctamente en el resumen del carrito."
            );
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void Examen()
        {
            InitialStepsForCompra();

            string nombre1 = "Galaxy S24 Ultra 512GB";
            string nombre2 = "ROG Phone 8 16GB";
            string color = "Negro";
            string nombre3 = "iPhone 15 Pro Max 256GB";
            string marca = "iPhone";
            string color2 = "Blanco";
            string precioEsperado = "1.499,00";
            string cantidadEsperada = "1";

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            var _detallePO = new DetailPurchase_PO(_driver, _output);

            string nombreUser = "elena@uclm.es";
            string apellidosUser = "Navarro Martínez";
            string direccionUser = "Avda. España 2, Albacete";

            // ====================================================================
            // ACT: Forzamos ambos parámetros en cada búsqueda para limpiar la caché
            // ====================================================================

            // 1. Filtramos por Color y buscamos el GalaxyS24 y el ROG
            _selectPO.SearchDevices(" ", "Negro");
            _selectPO.AddDeviceToCart(nombre1);
            _selectPO.AddDeviceToCart(nombre2);

            // 2. Filtramos por Nombre 
            _selectPO.SearchDevices("iPhone", " ");
            _selectPO.AddDeviceToCart(nombre3);


            _selectPO.RemoveDeviceFromCart("Galaxy S24 Ultra");
            _selectPO.RemoveDeviceFromCart("ROG Phone 8");
            _selectPO.ProceedToCheckout();

            // 5. Rellenamos el formulario de envío
            crearCompraPO.EscribirNombre(nombreUser);
            crearCompraPO.EscribirApellidos(apellidosUser);
            crearCompraPO.EscribirDireccion(direccionUser);
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            // 6. Verificaciones del recibo final
            string precioTotalEsperado = "1.,00 €";
            string fechaEsperada = DateTime.Now.ToString("dd/MM/yyyy");

            Assert.True(_detallePO.VerificarDetallesCabecera(
               $"{nombreUser} {apellidosUser}",
               direccionUser,
               fechaEsperada,
               precioTotalEsperado),
               "Los datos de la cabecera del detalle (Nombre, Dirección, Pago o Precio) son incorrectos.");

            List<string[]> dispositivosEsperados = new List<string[]>
            {
                new string[] { "iPhone 15 Pro Max", marca, color2, precioEsperado, cantidadEsperada}
            };

            Assert.True(
                _detallePO.CheckListOfDispositivos(dispositivosEsperados),
                $"El dispositivo '{nombre3}' no aparece en la tabla de detalles."
            );
        }
    }
}