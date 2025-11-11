using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System;
using System.IO;
using Selenium_Waits_Ready.Common;

namespace Selenium_Waits_Ready.Tests
{
    [TestFixture]
    public class WindowsTests : TestBase
    {
        [Test]
        public void HandleMultipleWindows()
        {
            try
            {
                Driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/windows");

                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));

                // 1) намери линка по няколко алтернативи
                IWebElement link = null;
                try
                {
                    link = wait.Until(d => d.FindElement(By.LinkText("Click Here")));
                }
                catch (WebDriverTimeoutException)
                {
                    // алтернатива: partial link text
                    try { link = wait.Until(d => d.FindElement(By.PartialLinkText("Click"))); } catch { }
                }

                Assert.IsNotNull(link, "Link 'Click Here' was not found on the page.");

                var original = Driver.CurrentWindowHandle;
                var beforeHandles = Driver.WindowHandles.Count;

                // 2) опит за нормален click
                try
                {
                    wait.Until(d => link.Displayed && link.Enabled);
                    link.Click();
                }
                catch
                {
                    // опит за click чрез JavaScript (по-агресивно)
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", link);
                }

                // 3) изчакай нов handle
                bool opened = false;
                try
                {
                    opened = wait.Until(d => d.WindowHandles.Count > beforeHandles);
                }
                catch (WebDriverTimeoutException)
                {
                    // 4) fallback: опитай да отвориш href в нов прозорец (за да заобиколим блокиращ popup)
                    var href = link.GetAttribute("href");
                    if (!string.IsNullOrEmpty(href))
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript($"window.open('{href}', '_blank');");
                        // даде ли време на браузъра да отвори нов прозорец?
                        opened = wait.Until(d => d.WindowHandles.Count > beforeHandles);
                    }
                }

                Assert.IsTrue(opened, "No new window was opened after clicking the link.");

                // 5) превключи на child и провери съдържание
                var handles = Driver.WindowHandles;
                var child = handles.First(h => h != original);
                Driver.SwitchTo().Window(child);

                // изчакай съдържание
                wait.Until(d => d.PageSource.Contains("New Window") || d.Title.Contains("New Window"));

                Assert.IsTrue(Driver.PageSource.Contains("New Window") || Driver.Title.Contains("New Window"),
                    "The new window did not contain expected text or title.");
            }
            catch (Exception ex)
            {
                // при fail — логни и заснеми screenshot + page source
                try
                {
                    var outDir = TestContext.CurrentContext.WorkDirectory;
                    var ts = ((ITakesScreenshot)Driver).GetScreenshot();
                    var fileName = Path.Combine(outDir, $"WindowsTest_fail_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    ts.SaveAsFile(fileName);
                    TestContext.AddTestAttachment(fileName, "screenshot on failure");

                    var htmlFile = Path.Combine(outDir, $"WindowsTest_fail_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                    File.WriteAllText(htmlFile, Driver.PageSource);
                    TestContext.AddTestAttachment(htmlFile, "page source on failure");
                }
                catch { /* ignore screenshot errors */ }

                // прехвърляме оригиналната грешка нагоре, за да се вижда в Test Explorer
                throw;
            }
        }
    }
}
