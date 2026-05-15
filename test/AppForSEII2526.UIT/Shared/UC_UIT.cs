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
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Maximizar para asegurar que los botones son visibles
            _driver.Manage().Window.Maximize();
        }

        protected void Initial_step_opening_the_web_page()
        {
            _driver.Navigate().GoToUrl(_URI);
        }

        protected void Perform_login(string email, string password)
        {
            _driver.Navigate().GoToUrl(_URI + "Account/Login");

            // Localizadores limpios por Nombre (más estables que XPath)
            _driver.FindElement(By.Name("Input.Email")).SendKeys(email);
            _driver.FindElement(By.Name("Input.Password")).SendKeys(password);

            // Click en el botón de Login (si el XPath cambia, fallará; mejor usar ID si el botón lo tiene)
            _driver.FindElement(By.XPath("//button[@type='submit']")).Click();
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
            // Quit cierra todas las ventanas y mata el proceso del driver
            _driver?.Quit();
            _driver?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}