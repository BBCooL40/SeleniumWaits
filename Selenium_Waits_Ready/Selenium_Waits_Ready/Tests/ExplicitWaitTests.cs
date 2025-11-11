using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Selenium_Waits_Ready.Common;
namespace Selenium_Waits_Ready.Tests
{
    [TestFixture]
    public class ExplicitWaitTests : TestBase
    {
        [Test]
        public void WaitForElement_VisibleThenClick()
        {
            Driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/1");
            Driver.FindElement(By.CssSelector("#start button")).Click();
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            var el = wait.Until(d => {
                var e = d.FindElement(By.CssSelector("#finish h4"));
                return (e.Displayed ? e : null);
            });
            Assert.That(el.Text.Contains("Hello"));
        }
    }
}
