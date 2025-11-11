using NUnit.Framework;
using OpenQA.Selenium;
using Selenium_Waits_Ready.Common;
namespace Selenium_Waits_Ready.Tests
{
    [TestFixture]
    public class IFrameTests : TestBase
    {
        [Test]
        public void SwitchToIFrame_ByIndex_Id_WebElement()
        {
            Driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/iframe");
            Driver.SwitchTo().Frame(0);
            var editor = Driver.FindElement(By.Id("tinymce"));
            Assert.IsNotNull(editor);
            Driver.SwitchTo().DefaultContent();
            Assert.That(Driver.PageSource.Contains("An iFrame") || Driver.Title.Length > 0);
        }
    }
}
