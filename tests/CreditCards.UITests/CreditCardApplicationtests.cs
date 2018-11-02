using System;
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

        public void Dispose()
        {
            // close any open web browser instance and dispose of it
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
