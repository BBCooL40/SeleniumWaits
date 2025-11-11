using OpenQA.Selenium;
namespace Selenium_Waits_Ready.Pages
{
    public class QuickFindPage
    {
        private readonly IWebDriver _driver;
        public QuickFindPage(IWebDriver driver) => _driver = driver;
        public IWebElement SearchInput => _driver.FindElement(By.Name("keywords"));
        public IWebElement SearchButton => _driver.FindElement(By.CssSelector("input[title=' Quick Find '],[alt='Quick Find']"));
    }
}
