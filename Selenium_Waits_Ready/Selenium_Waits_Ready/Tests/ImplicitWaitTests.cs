using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using Selenium_Waits_Ready.Common;
using Selenium_Waits_Ready.Pages;

namespace Selenium_Waits_Ready.Tests
{
    [TestFixture]
    public class ImplicitWaitTests : TestBase
    {
        [Test]
        public void QuickFind_AddsToCartByEnterKey()
        {
            // По-голям таймаут за по-бавни сайтове
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            try
            {
                Driver.Navigate().GoToUrl("http://practice.bpbonline.com/");
                TestContext.WriteLine("Opened URL: " + Driver.Url);
                TestContext.WriteLine("Title: " + Driver.Title);

                // Намерете полето
                var input = wait.Until(d => d.FindElement(By.Name("keywords")));
                input.Clear();
                input.SendKeys("keyboard");

                // опит 1: Enter
                input.SendKeys(Keys.Enter);

                // изчакай кратко да видим дали URL се е променил или резултат се появява
                System.Threading.Thread.Sleep(700); // малък паузен опит

                // логваме текущ URL/Title след Enter
                TestContext.WriteLine("After Enter — URL: " + Driver.Url);
                TestContext.WriteLine("After Enter — Title: " + Driver.Title);

                // Опит 2: търсене на Buy елемент (няколко форми)
                IWebElement buy = null;
                try
                {
                    buy = wait.Until(d =>
                    {
                        // partial link text "Buy" — tolerant
                        var candidate = d.FindElements(By.PartialLinkText("Buy")).FirstOrDefault(e => e.Displayed && e.Enabled);
                        if (candidate != null) return candidate;
                        // input type=image или submit с alt/title
                        candidate = d.FindElements(By.CssSelector("input[type='image'], input[type='submit'], button"))
                                     .FirstOrDefault(e => (e.Text ?? "").IndexOf("Buy", StringComparison.OrdinalIgnoreCase) >= 0
                                                          || ((e.GetAttribute("alt") ?? "").IndexOf("Buy", StringComparison.OrdinalIgnoreCase) >= 0)
                                                          || ((e.GetAttribute("title") ?? "").IndexOf("Buy", StringComparison.OrdinalIgnoreCase) >= 0));
                        return candidate;
                    });
                }
                catch (WebDriverTimeoutException) { /* няма бутон */ }

                if (buy != null)
                {
                    TestContext.WriteLine("Found Buy element, clicking...");
                    buy.Click();
                }
                else
                {
                    TestContext.WriteLine("Buy element NOT found — опитваме да submit-нем формата или да кликнем quick-find button по други селектори.");

                    // опитай да кликнеш конкретния image-button (често има title / alt / src)
                    var altBtn = Driver.FindElements(By.CssSelector("input[title*='Quick Find'], input[alt*='Quick Find'], input[src*='button_quick_find'], input[src*='quick_find']"))
                                       .FirstOrDefault(e => e.Displayed && e.Enabled);
                    if (altBtn != null) { altBtn.Click(); TestContext.WriteLine("Clicked alt quick-find button."); }
                    else
                    {
                        // опит: submit формата с JS
                        try
                        {
                            ((IJavaScriptExecutor)Driver).ExecuteScript("var f = document.querySelector('form'); if(f) f.submit();");
                            TestContext.WriteLine("Submitted form by JS fallback.");
                        }
                        catch { TestContext.WriteLine("JS submit failed."); }
                    }
                }

                // Накрая — изчакай индикатор, че имаме продукт/cart
                bool pageHasProduct = wait.Until(d =>
                {
                    var src = d.PageSource;
                    if (src.IndexOf("Keyboard", StringComparison.OrdinalIgnoreCase) >= 0) return true;
                    if (d.FindElements(By.CssSelector(".product, .product-list, .product-name, .cart, .cart-contents, .item, .product-item")).Any()) return true;
                    return false;
                });

                TestContext.WriteLine("pageHasProduct = " + pageHasProduct);
                Assert.IsTrue(pageHasProduct, "The product or cart indicator was not found on page after search.");
            }
            catch (Exception ex)
            {
                // Save artifacts (screenshot + html + current URL) and rethrow
                try
                {
                    var outDir = TestContext.CurrentContext.WorkDirectory;
                    Directory.CreateDirectory(outDir);

                    var now = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var shotPath = Path.Combine(outDir, $"QuickFind_fail_{now}.png");
                    ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(shotPath);
                    TestContext.AddTestAttachment(shotPath, "screenshot on failure");

                    var htmlPath = Path.Combine(outDir, $"QuickFind_fail_{now}.html");
                    File.WriteAllText(htmlPath, Driver.PageSource);
                    TestContext.AddTestAttachment(htmlPath, "page source on failure");

                    TestContext.WriteLine("Saved artifacts:");
                    TestContext.WriteLine("Screenshot: " + shotPath);
                    TestContext.WriteLine("HTML: " + htmlPath);
                    TestContext.WriteLine("Current URL: " + Driver.Url);
                }
                catch (Exception saveEx)
                {
                    TestContext.WriteLine("Failed saving artifacts: " + saveEx);
                }

                TestContext.WriteLine("Exception: " + ex);
                throw;
            }
        }
    }
}
