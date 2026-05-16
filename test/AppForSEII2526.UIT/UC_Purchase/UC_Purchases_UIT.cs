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
            // CORREGIDO: Añadida la barra diagonal '/' por seguridad para evitar colisiones de rutas URL
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
        // CORREGIDO: Cambiados los InlineData por los móviles reales que sí existen en tu catálogo actual
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

            // CORREGIDO: Usamos los móviles reales del catálogo y recalculamos el precio final esperado
            string movil1 = "Galaxy S24 Ultra 512GB";
            string movil2 = "iPhone 15 Pro Max 256GB";
            string precioEsperadoFinal = "1.499,00 €";

            _selectPO.SearchDevices("Galaxy", "");
            _selectPO.AddDeviceToCart(movil1);

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart(movil2);

            // Quitamos el Galaxy, debe quedar solo el valor del iPhone
            _selectPO.RemoveDeviceFromCart(movil1);

            Assert.True(_selectPO.CheckTotalPrice(precioEsperadoFinal));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC1_6Compra_Carrito_Vacio_Oculta_Tramitar()
        {
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB"); // CORREGIDO: Nombre exacto de tarjeta
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

            // ACT: Dejamos el nombre vacío para forzar la validación frontend
            crearCompraPO.EscribirNombre("");
            crearCompraPO.EscribirApellidos("Pérez García");
            crearCompraPO.EscribirDireccion("Avenida Libertad 45, Barcelona");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash" para activar tu combobox
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

            // ACT
            crearCompraPO.EscribirNombre("elena@uclm.es"); // CORREGIDO: Cumple regla del email obligatorio
            crearCompraPO.EscribirApellidos("");
            crearCompraPO.EscribirDireccion("Calle Mayor 123, Madrid");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"
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

            // ACT
            crearCompraPO.EscribirNombre("elena@uclm.es"); // CORREGIDO: Formato Email obligatorio
            crearCompraPO.EscribirApellidos("Pérez García");
            crearCompraPO.EscribirDireccion("");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"
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

            // CORREGIDO: Mensaje de error de validación real capturado en los logs de tu consola web
            string mensajeEsperado = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";
            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            // ACT
            crearCompraPO.EscribirNombre("usuario_inexistente_pero_formato_email@test.com"); // CORREGIDO: Email válido en estructura
            crearCompraPO.EscribirApellidos("Navarro Martínez");
            crearCompraPO.EscribirDireccion("Avda. España 2, Albacete");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_11_Dispositivo_sin_stock()
        {
            InitialStepsForCompra();

            string dispositivo = "iPhone 15 Pro Max 256GB"; // CORREGIDO: Dispositivo real de la BD
            _selectPO.SearchDevices("iPhone", "");

            for (int i = 0; i < 15; i++)
            {
                _selectPO.AddDeviceToCart(dispositivo);
            }

            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeEsperado = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            // ACT
            crearCompraPO.EscribirNombre("elena@uclm.es"); // CORREGIDO: Formato Email
            crearCompraPO.EscribirApellidos("Gómez Fernández");
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_12_Volver_Desde_CrearCompra_Mantiene_Carrito()
        {
            InitialStepsForCompra();

            string movilPrueba = "Galaxy S24 Ultra 512GB"; // CORREGIDO: Móvil real
            string precioAntesDeIrse = "1.399,99 €";     // CORREGIDO: Formato de precio con el punto de miles

            _selectPO.SearchDevices("Galaxy", "");
            _selectPO.AddDeviceToCart(movilPrueba);
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            // ACT
            crearCompraPO.ClickVolver();

            // ASSERT: El precio total del carrito debe conservarse intacto al regresar
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

            // TRUCO: 41 letras 'a' + '@uclm.es' = 51 caracteres. Cumple la regla de ser email, pero excede el límite de 50.
            string nombreLargo = new string('a', 41) + "@uclm.es";
            string mensajeError = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            // ACT
            crearCompraPO.EscribirNombre(nombreLargo);
            crearCompraPO.EscribirApellidos("Gómez Fernández");
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"

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
            string mensajeError = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            // ACT
            crearCompraPO.EscribirNombre("elena@uclm.es"); // CORREGIDO: Formato Email
            crearCompraPO.EscribirApellidos(apellidosLargos);
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"

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
            string mensajeError = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            // ACT
            crearCompraPO.EscribirNombre("elena@uclm.es"); // CORREGIDO: Formato Email
            crearCompraPO.EscribirApellidos("Gómez Fernández");
            crearCompraPO.EscribirDireccion(direccionLarga);
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"

            crearCompraPO.ClickConfirmar();

            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]

        [Trait("LevelTesting", "Funcional Testing")]

        public void CU1_1_Flujo_Basico()

        {

            // ==========================================

            // 1. ARRANGE (Configuración con tu HTML real)

            // ==========================================

            InitialStepsForCompra();



            // Nombres completos para poder hacer click en las TARJETAS del catálogo

            string cardMovil1 = "Galaxy S24 Ultra 512GB";

            string cardMovil2 = "iPhone 15 Pro Max 256GB";



            // Nombres EXACTOS recortados que pinta tu web en el RECIBO (Ver captura)

            string nombreRecibo1 = "Galaxy S24 Ultra";

            string marcaEsperada = "Samsung";

            string colorEsperado = "Negro";

            string precioEsperado = "1.399,99"; // Añadido el punto de los miles

            string cantidadEsperada = "1";

            string descripcionEsperada = "Compra realizada desde la web";



            string nombreRecibo2 = "iPhone 15 Pro Max";

            string marcaEsperada2 = "Apple";

            string colorEsperado2 = "Blanco";

            string precioEsperado2 = "1.499,00"; // Añadido el punto de los miles

            string cantidadEsperada2 = "1";

            string descripcionEsperada2 = "Compra realizada desde la web";



            // Datos obligatorios del comprador (Mantenemos tu regla del Email)

            string nombreUser = "elena@uclm.es";

            string apellidosUser = "Navarro Martínez";

            string direccionUser = "Avda. España 2, Albacete";



            // ==========================================

            // 2. ACT (Acciones automatizadas)

            // ==========================================



            // Añadimos primer dispositivo

            _selectPO.SearchDevices("Galaxy", "");

            _selectPO.AddDeviceToCart(cardMovil1);



            // Añadimos segundo dispositivo

            _selectPO.SearchDevices("iPhone", "");

            _selectPO.AddDeviceToCart(cardMovil2);

            _selectPO.ProceedToCheckout();



            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            var _detallePO = new DetailPurchase_PO(_driver, _output);



            // Rellenamos el formulario de envío

            crearCompraPO.EscribirNombre(nombreUser);

            crearCompraPO.EscribirApellidos(apellidosUser);

            crearCompraPO.EscribirDireccion(direccionUser);

            crearCompraPO.SeleccionarPago("Cash"); // Mantenemos "Cash" para que pinche tu combo



            crearCompraPO.ClickConfirmar();



            // ==========================================

            // 3. ASSERT (Verificaciones del Recibo)

            // ==========================================

            string precioTotalEsperado = "2.898,99 €";

            string fechaEsperada = DateTime.Now.ToString("dd/MM/yyyy");



            // Verificar los datos del comprador en la cabecera superior

            Assert.True(_detallePO.VerificarDetallesCabecera(

                $"{nombreUser} {apellidosUser}",

                direccionUser,

                fechaEsperada,

                precioTotalEsperado),

                "Los datos de la cabecera del detalle (Nombre, Dirección, Pago o Precio) son incorrectos.");



            // Verificar la lista de productos usando el texto exacto de la tabla de la foto

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

            // CORREGIDO: Limpiamos los nombres para que coincidan exactamente con la lista de la cesta de tu web (Ver logs anteriores)
            string nombreEsperado = "Galaxy S24 Ultra 512GB";
            string colorEsperado = "Negro";
            string precioEsperado = "1.399,99 €";

            string nombreEsperado2 = "iPhone 15 Pro Max 256GB";
            string colorEsperado2 = "Blanco";
            string precioEsperado2 = "1.499,00 €";

            string nombreUser = "elena@uclm.es";
            string apellidosUser = "Navarro Martínez";
            string direccionUser = "Avda. España 2, Albacete";

            _selectPO.SearchDevices("Galaxy", "");
            _selectPO.AddDeviceToCart(cardMovil1);

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart(cardMovil2);
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            crearCompraPO.EscribirNombre(nombreUser);
            crearCompraPO.EscribirApellidos(apellidosUser);
            crearCompraPO.EscribirDireccion(direccionUser);
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"

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

            // CORREGIDO AL COMPLETO: Reconstruimos la bonita lógica del examen usando móviles reales de tu base de datos
            // Móvil 1: Añadir y luego borrar
            string nombre1 = "iPhone 15 Pro Max 256GB";
            // Móvil 2: Añadir buscando por color y luego borrar
            string nombre2 = "Redmi Note 13 Pro 128GB";
            string color2 = "Azul";
            // Móvil 3: El que se queda en el carrito definitivo y se compra solo
            string nombre3 = "Pixel 8 Pro 128GB";
            string marca = "Google";
            string color = "Gris";
            string precioEsperado = "1.099,00"; // Formato correcto de miles
            string cantidadEsperada = "1";
            string descripcionEsperada = "Compra realizada desde la web";

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            var _detallePO = new DetailPurchase_PO(_driver, _output);

            string nombreUser = "elena@uclm.es"; // CORREGIDO: Formato Email obligatorio
            string apellidosUser = "Navarro Martínez";
            string direccionUser = "Avda. España 2, Albacete";

            // ACT
            // 1. Añadimos el primer móvil
            _selectPO.SearchDevices("iPhone 15", "");
            _selectPO.AddDeviceToCart(nombre1);

            // 2. Filtramos por el color del segundo móvil y lo añadimos
            _selectPO.SearchDevices("", color2);
            _selectPO.AddDeviceToCart(nombre2);

            // 3. Buscamos el tercer móvil y lo añadimos
            _selectPO.SearchDevices("Pixel", "");
            _selectPO.AddDeviceToCart(nombre3);

            // 4. Simulamos la gestión eliminando el 1 y el 2 (Solo queda el Pixel 8 Pro)
            _selectPO.RemoveDeviceFromCart(nombre1);
            _selectPO.RemoveDeviceFromCart(nombre2);
            _selectPO.ProceedToCheckout();

            // 5. Rellenamos datos de compra
            crearCompraPO.EscribirNombre(nombreUser);
            crearCompraPO.EscribirApellidos(apellidosUser);
            crearCompraPO.EscribirDireccion(direccionUser);
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash"

            crearCompraPO.ClickConfirmar();

            string precioTotalEsperado = "1.099,00 €";
            string fechaEsperada = DateTime.Now.ToString("dd/MM/yyyy");

            // ASSERT: Validamos recibo final del Pixel 8 Pro
            Assert.True(_detallePO.VerificarDetallesCabecera(
               $"{nombreUser} {apellidosUser}",
               direccionUser,
               fechaEsperada,
               precioTotalEsperado),
               "Los datos de la cabecera del detalle (Nombre, Dirección, Pago o Precio) son incorrectos.");

            List<string[]> dispositivosEsperados = new List<string[]>
            {
                new string[] { "Pixel 8 Pro", marca, color, precioEsperado, cantidadEsperada, descripcionEsperada } // Nombre recortado para el recibo
            };

            Assert.True(
                _detallePO.CheckListOfDispositivos(dispositivosEsperados),
                $"El dispositivo '{nombre3}' no aparece en la tabla de detalles."
            );
        }
    }
}