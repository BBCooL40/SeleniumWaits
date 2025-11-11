using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class TestBase
{
    protected IWebDriver driver;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();

        // Headless & CI-friendly args
        options.AddArgument("--headless=new"); // ??? "--headless=chrome" ?? ??-????? ??????
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-infobars");

        // ??? chromedriver/Chromium ?? ?? ???????????? ???, ????? ?? ?????? BinaryLocation:
        // options.BinaryLocation = "/usr/bin/chromium-browser";

        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Quit();
    }
}
