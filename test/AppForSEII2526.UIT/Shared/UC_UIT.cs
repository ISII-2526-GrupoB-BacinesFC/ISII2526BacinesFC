using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared
{
    public class UC_UIT : IDisposable
    {
        // Cambia a true si quieres que el navegador no se abra físicamente (segundo plano)
        private readonly bool _pipeline = false;
        private readonly string _browser = "Edge";

        protected IWebDriver _driver;
        protected readonly ITestOutputHelper _output;

        // URL base de tu aplicación
        public string _URI => "https://localhost:7081/";

        public UC_UIT(ITestOutputHelper output)
        {
            _output = output;

            switch (_browser)
            {
                case "Firefox":
                    SetUp_FireFox4UIT();
                    break;
                case "Edge":
                    SetUp_EdgeFor4UIT();
                    break;
                default:
                    SetUp_Chrome4UIT();
                    break;
            }

            // === CONFIGURACIÓN DE ESPERAS ===
            // Espera hasta 10 segundos a que los elementos aparezcan antes de dar error
            if (_driver != null)
            {
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                // Maximizar para asegurar que los botones son visibles
                _driver.Manage().Window.Maximize();
            }
        }

        protected void Initial_step_opening_the_web_page()
        {
            _driver?.Navigate().GoToUrl(_URI);
        }

        protected void Perform_login(string email, string password)
        {
            if (_driver == null) return;

            // 1. Construir la URL de forma segura (sin dobles barras)
            string loginUrl = $"{_URI.TrimEnd('/')}/Account/Login";
            _driver.Navigate().GoToUrl(loginUrl);

            // 2. ESPERA EXPLÍCITA: Espera hasta 10 segundos a que aparezca el campo de Email
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            try
            {
                var emailField = wait.Until(d => d.FindElement(By.Name("Input.Email")));
                emailField.Clear();
                emailField.SendKeys(email);

                _driver.FindElement(By.Name("Input.Password")).SendKeys(password);

                // En lugar de un XPath gigante, buscamos el botón de tipo 'submit' que es más estable
                _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception($"❌ No se cargó la página de login en: {loginUrl}. Revisa si el puerto 7081 está abierto.");
            }
        }

        protected void SetUp_Chrome4UIT()
        {
            var options = new ChromeOptions { AcceptInsecureCertificates = true };
            if (_pipeline) options.AddArgument("--headless");
            _driver = new ChromeDriver(options);
        }

        protected void SetUp_FireFox4UIT()
        {
            var options = new FirefoxOptions { AcceptInsecureCertificates = true };
            if (_pipeline) options.AddArgument("--headless");
            _driver = new FirefoxDriver(options);
        }

        protected void SetUp_EdgeFor4UIT()
        {
            var options = new EdgeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };

            // Detectar si estamos en el servidor (CI) o en local
            bool isServer = _pipeline || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));

            if (isServer)
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
            }

            _driver = new EdgeDriver(options);
        }

        public void Dispose()
        {
            // Libera y cierra el proceso de Edge y del Driver para que no se queden colgados en segundo plano
            try
            {
                _driver?.Quit();
                _driver?.Dispose();
            }
            catch (Exception ex)
            {
                _output?.WriteLine($"Advertencia al cerrar el navegador: {ex.Message}");
            }

            GC.SuppressFinalize(this);
        }
    }
}