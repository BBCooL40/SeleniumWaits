using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Selenium_Waits_Ready.Common
{
    public class TestBase
    {
        // Защитено свойство, достъпно за наследяване в тестовете
        protected IWebDriver Driver { get; private set; }

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();

            // Headless & CI friendly конфигурация
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-infobars");

            // 🔑 Това е критично за GitHub Actions (Ubuntu runner)
            options.BinaryLocation = "/usr/bin/chromium-browser";

            Driver = new ChromeDriver(options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                Driver?.Quit();
            }
            catch
            {
                // Игнорираме евентуални грешки при затваряне на браузъра
            }
        }
    }
}
