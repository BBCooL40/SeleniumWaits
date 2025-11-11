using NUnit.Framework;
using OpenQA.Selenium;
using Selenium_Waits_Ready.Common;
namespace Selenium_Waits_Ready.Tests
{
    [TestFixture]
    public class AlertsTests : TestBase
    {
        [Test]
        public void AcceptAlert_ShowsResult()
        {
            Driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/javascript_alerts");
            Driver.FindElement(By.XPath("//button[text()='Click for JS Alert']")).Click();
            var alert = Driver.SwitchTo().Alert();
            alert.Accept();
            Assert.That(Driver.PageSource.Contains("You successfully clicked an alert"));
        }
    }
}
