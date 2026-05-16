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
        private SelectDevices_PO _selectPO; //  Nombre correcto

        public UC_Purchases_UIT(ITestOutputHelper output) : base(output)
        {
            _selectPO = new SelectDevices_PO(_driver, _output); //  Nombre correcto
        }

        private void InitialStepsForCompra()
        {

            _driver.Navigate().GoToUrl(_URI + "Purchases/SelectDevices");
        }

        // PRUEBAS DEL SELECT DISPOSITIVOS COMPRAR

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_2_No_hay_Dispositivos()
        {
            // ARRANGE
            InitialStepsForCompra();

            // Definimos el texto "sajfbuf" y el mensaje esperado
            string colorInexistente = "sajfbuf";
            string mensajeEsperado = "No se han encontrado dispositivos con esos filtros.";

            // ACT

            _selectPO.SearchDevices("", colorInexistente);

            // ASSERT 


            Assert.True(_selectPO.CheckMessageErrorNotAvaibleDevices(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");


        }

        [Theory]
        [Trait("LevelTesting", "Funcional Testing")]
        // Caso 1: Filtrar por Nombre CU1_3
        [InlineData("Oppo", "", "Oppo Find X5 Pro 256GB", "Oppo", "799,99 €")]
        // Caso 2: Filtrar por Color CU1_4
        [InlineData("", "Plata", "iPhone 14 Pro 512GB", "Apple", "1.399,99 €")]
        public void UC1_3Y4_Compra_FiltrarDispositivos(string filtroNombre, string filtroColor, string nombreEsperado, string marcaEsperada, string precioEsperado)
        {
            // Arrange
            InitialStepsForCompra();

            var expectedDispositivos = new List<string[]>
            {
                new string[] { nombreEsperado, marcaEsperada, precioEsperado }
            };

            // Act
            _selectPO.SearchDevices(filtroNombre, filtroColor);

            // Assert
            Assert.True(_selectPO.CheckListOfDevices(expectedDispositivos),
                $"Error: No se encontró la tarjeta con: {nombreEsperado} | {marcaEsperada}");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_5_GestionCarrito_RecalculoPrecios()
        {

            // ARRANGE
            InitialStepsForCompra();

            string movil1 = "Oppo";
            string movil2 = "iPhone 14 Pro 512GB";

            string precioEsperadoTotalAmbos = "2.199,98 €";
            string precioEsperadoFinal = "1.399,99 €";

            // ACT 

            _selectPO.SearchDevices("Oppo", "");
            _selectPO.AddDeviceToCart(movil1);

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart(movil2);

            _selectPO.RemoveDeviceFromCart(movil1);

            // ASSERT 
            Assert.True(_selectPO.CheckTotalPrice(precioEsperadoFinal));

        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC1_6Compra_Carrito_Vacio_Oculta_Tramitar()
        {
            //  ARRANGE
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");

            _selectPO.AddDeviceToCart("iPhone");


            // ACT 

            _selectPO.EmptyCart();


            // ASSERT 

            Assert.True(_selectPO.IsProceedToCheckoutHidden(), "El botón 'Tramitar Pedido' debería ocultarse tras vaciar el carrito.");
        }


        //PRUEBAS DEL CREAR COMPRA

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_7_Nombre_Vacio()
        {
            //  ARRANGE 
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeError = "Por favor, introduce tu Nombre.";

            // ACT 

            crearCompraPO.EscribirNombre("");
            crearCompraPO.EscribirApellidos("Pérez García");
            crearCompraPO.EscribirDireccion("Avenida Libertad 45, Barcelona");
            crearCompraPO.SeleccionarPago("Cash");
            crearCompraPO.ClickConfirmar();

            // ASSERT 
            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_8_Apellidos_Vacio()
        {
            // ARRANGE 
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 14 Pro 512GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeError = "Por favor, introduce tus Apellidos.";

            // ACT 
            crearCompraPO.EscribirNombre("Juan");
            crearCompraPO.EscribirApellidos("");
            crearCompraPO.EscribirDireccion("Calle Mayor 123, Madrid");
            crearCompraPO.SeleccionarPago("Efectivo");

            crearCompraPO.ClickConfirmar();

            // ASSERT 
            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_9_Direccion_Vacio()
        {
            // ARRANGE
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 14 Pro 512GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string mensajeError = "Es obligatorio introducir una Dirección de entrega.";

            // ACT 
            crearCompraPO.EscribirNombre("Juan");
            crearCompraPO.EscribirApellidos("Pérez García");
            crearCompraPO.EscribirDireccion("");
            crearCompraPO.SeleccionarPago("Efectivo");

            crearCompraPO.ClickConfirmar();

            // ASSERT 
            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_10_Usuario_No_Existe()
        {
            //  ARRANGE 
            InitialStepsForCompra();


            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            string mensajeEsperado = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";
            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            //  ACT 
            crearCompraPO.EscribirNombre("x");
            crearCompraPO.EscribirApellidos("x");
            crearCompraPO.EscribirDireccion("x");
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            // ASSERT 
            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");

        }
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_11_Dispositivo_sin_stock()
        {
            // ARRANGE
            InitialStepsForCompra();


            string dispositivo = "iPhone 15 Pro Max 256GB";
            _selectPO.SearchDevices("iPhone", "");

            for (int i = 0; i < 15; i++)
            {
                _selectPO.AddDeviceToCart(dispositivo);
            }

            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            string mensajeEsperado = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            // ACT 
            crearCompraPO.EscribirNombre("elena@uclm.es");
            crearCompraPO.EscribirApellidos("Navarro Martínez");
            crearCompraPO.EscribirDireccion("Avda. España, Albacete");
            crearCompraPO.SeleccionarPago("Cash");

            crearCompraPO.ClickConfirmar();

            // ASSERT 
            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeEsperado), "El mensaje de error debería ser visible en pantalla.");



        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_12_Volver_Desde_CrearCompra_Mantiene_Carrito()
        {
            // ARRANGE 
            InitialStepsForCompra();

            string movilPrueba = "Oppo";

            _selectPO.SearchDevices("Oppo", "");
            _selectPO.AddDeviceToCart(movilPrueba);

            string precioAntesDeIrse = "799,99 €";

            _selectPO.ProceedToCheckout();


            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            //  ACT 

            crearCompraPO.ClickVolver();


            // ASSERT 


            Assert.True(_selectPO.CheckTotalPrice(precioAntesDeIrse));



        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_13_Nombre_Excede_Longitud()
        {
            // ARRANGE 
            InitialStepsForCompra();

            // Usamos el móvil real de tu catálogo actual
            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            // TRUCO: 41 letras 'a' + '@uclm.es' = 51 caracteres (Es un email válido pero demasiado largo)
            string nombreLargo = new string('a', 41) + "@uclm.es";
            string mensajeError = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            // ACT 
            crearCompraPO.EscribirNombre(nombreLargo);
            crearCompraPO.EscribirApellidos("Gómez Fernández");
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash" en vez de "Efectivo"

            crearCompraPO.ClickConfirmar();

            // ASSERT 
            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_14_Apellidos_Excede_Longitud()
        {
            // ARRANGE 
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string apellidosLargos = new string('a', 71); // Excede los 70 máximos
            string mensajeError = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            //  ACT 
            crearCompraPO.EscribirNombre("elena@uclm.es"); // Tu regla del email obligatorio
            crearCompraPO.EscribirApellidos(apellidosLargos);
            crearCompraPO.EscribirDireccion("Paseo de la Castellana 100, Madrid");
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash" en vez de "Efectivo"

            crearCompraPO.ClickConfirmar();

            // ASSERT 
            Assert.True(crearCompraPO.CheckMessageErrorNotAvaibleMovies(mensajeError), "El mensaje de error debería ser visible en pantalla.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_15_Direccion_Excede_Longitud()
        {
            //  ARRANGE
            InitialStepsForCompra();

            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart("iPhone 15 Pro Max 256GB");
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            string direccionLarga = new string('a', 101); // Excede los 100 máximos
            string mensajeError = "Atención: ERROR DE VALIDACIÓN (400): Revisa que no haya campos vacíos en los ítems.";

            // ACT 
            crearCompraPO.EscribirNombre("elena@uclm.es"); // Tu regla del email obligatorio
            crearCompraPO.EscribirApellidos("Gómez Fernández");
            crearCompraPO.EscribirDireccion(direccionLarga);
            crearCompraPO.SeleccionarPago("Cash"); // CORREGIDO: "Cash" en vez de "Efectivo"

            crearCompraPO.ClickConfirmar();

            // ASSERT 
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
            // ==========================================
            // 1. ARRANGE (Móviles reales de tu catálogo)
            // ==========================================
            InitialStepsForCompra();

            // Nombres exactos de las tarjetas para poder añadirlos al carrito
            string cardMovil1 = "Galaxy S24 Ultra 512GB";
            string cardMovil2 = "iPhone 15 Pro Max 256GB";

            // CORREGIDO: Quitamos las marcas "Samsung" y "Apple" porque tu web no las pinta ahí (Ver logs)
            string nombreEsperado = "Galaxy S24 Ultra 512GB";
            string colorEsperado = "Negro";
            string precioEsperado = "1.399,99 €";

            string nombreEsperado2 = "iPhone 15 Pro Max 256GB";
            string colorEsperado2 = "Blanco";
            string precioEsperado2 = "1.499,00 €";

            // Datos del comprador (Obligatorio formato Email)
            string nombreUser = "elena@uclm.es";
            string apellidosUser = "Navarro Martínez";
            string direccionUser = "Avda. España 2, Albacete";

            // ==========================================
            // 2. ACT (Navegación y rellenado de formulario)
            // ==========================================

            // Buscamos y añadimos el Galaxy
            _selectPO.SearchDevices("Galaxy", "");
            _selectPO.AddDeviceToCart(cardMovil1);

            // Buscamos y añadimos el iPhone
            _selectPO.SearchDevices("iPhone", "");
            _selectPO.AddDeviceToCart(cardMovil2);

            // Vamos a la pantalla de tramitar
            _selectPO.ProceedToCheckout();

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);

            // Rellenamos los campos del formulario
            crearCompraPO.EscribirNombre(nombreUser);
            crearCompraPO.EscribirApellidos(apellidosUser);
            crearCompraPO.EscribirDireccion(direccionUser);
            crearCompraPO.SeleccionarPago("Cash"); // Mantenemos "Cash" para activar tu combo

            // ==========================================
            // 3. ASSERT (Verificación de la cesta antes de comprar)
            // ==========================================
            List<string[]> dispositivosEsperados = new List<string[]>
    {
        new string[] { nombreEsperado, colorEsperado, precioEsperado },
        new string[] { nombreEsperado2, colorEsperado2, precioEsperado2 }
    };

            // Comprobamos que la lista lateral/resumen tiene los dos teléfonos con sus datos reales
            Assert.True(
                crearCompraPO.CheckListOfDispositivosEnCarrito(dispositivosEsperados),
                "Los dispositivos o sus precios no se muestran correctamente en el resumen del carrito."
            );
        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void Examen()
        {


            //  ARRANGE 
            InitialStepsForCompra();
            string nombre1 = "iPhone 14";

            string nombre2 = "Galaxy";
            string color2 = "Verde";

            string nombre3 = "iPhone 13";
            string marca = "Apple";
            string color = "Azul";
            string precioEsperado = "799,99";
            string cantidadEsperada = "1";
            string descripcionEsperada = "Compra Web";

            var crearCompraPO = new CreatePurchase_PO(_driver, _output);
            var _detallePO = new DetailPurchase_PO(_driver, _output);



            string nombreUser = "Juan";
            string apellidosUser = "Pérez García";
            string direccionUser = "Calle Mayor 123";

            //  ACT 

            _selectPO.SearchDevices(nombre1, "");
            _selectPO.AddDeviceToCart(nombre1);

            _selectPO.SearchDevices("", color2);
            _selectPO.AddDeviceToCart(nombre2);
            _selectPO.SearchDevices(nombre3, "");
            _selectPO.AddDeviceToCart(nombre3);

            _selectPO.RemoveDeviceFromCart(nombre1);
            _selectPO.RemoveDeviceFromCart(nombre2);
            _selectPO.ProceedToCheckout();


            crearCompraPO.EscribirNombre(nombreUser);
            crearCompraPO.EscribirApellidos(apellidosUser);
            crearCompraPO.EscribirDireccion(direccionUser);
            crearCompraPO.SeleccionarPago("Efectivo");

            crearCompraPO.ClickConfirmar();

            string precioTotalEsperado = "799,99 €";
            string fechaEsperada = DateTime.Now.ToString("dd/MM/yyyy");

            Assert.True(_detallePO.VerificarDetallesCabecera(
               $"{nombreUser} {apellidosUser}",
               direccionUser,
               fechaEsperada,
               precioTotalEsperado),
               "Los datos de la cabecera del detalle (Nombre, Dirección, Pago o Precio) son incorrectos.");

            List<string[]> dispositivosEsperados = new List<string[]>
            {
                new string[] { nombre3, marca, color, precioEsperado,cantidadEsperada,descripcionEsperada }
            };

            Assert.True(
                _detallePO.CheckListOfDispositivos(dispositivosEsperados),
                $"El dispositivo '{nombre3}' no aparece en la tabla de detalles."
            );





        }
    }
}
