using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace CreditCards.UITests
{
    public class CreditCardApplicationTests : IDisposable
    {
        private readonly IWebDriver _driver;

        public CreditCardApplicationTests()
        {
            _driver = new ChromeDriver();
        }

        [Fact]
        public void ShouldLoadApplicationPage_SmokeTest()
        {
            _driver.Navigate().GoToUrl("http://localhost:44108/apply");

            Assert.Equal("Credit Card Application - CreditCards", _driver.Title);
        }

        [Fact]
        public void ShouldValidateApplicationDetails()
        {
            _driver.Navigate().GoToUrl("http://localhost:44108/Apply");

            // Don't enter a first name

            IWebElement lastName = _driver.FindElement(By.Name("LastName"));
            lastName.SendKeys("Smith");

            DelayForDemoVideo();

            IWebElement frequentFlyerNum = _driver.FindElement(By.Id("FrequentFlyerNumber"));
            frequentFlyerNum.SendKeys("012345-A");

            DelayForDemoVideo();

            _driver.FindElement(By.Id("Age")).SendKeys("18");

            DelayForDemoVideo();

            _driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys("100000");

            DelayForDemoVideo();

            _driver.FindElement(By.Id("submitApplication")).Click();

            Assert.Equal("Credit Card Application - CreditCards", _driver.Title);

            IWebElement firstErrorMessage =
                _driver.FindElement(By.CssSelector(".validation-summary-errors ul > li"));

            Assert.Equal("Please provide a first name", firstErrorMessage.Text);
        }

        /// <summary>
        /// Brief delay to slow down browser interactions for
        /// demo video recording purposes
        /// </summary>
        private static void DelayForDemoVideo()
        {
            Thread.Sleep(500);
        }

        public void Dispose()
        {
            // close any open web browser instance and dispose of it
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
