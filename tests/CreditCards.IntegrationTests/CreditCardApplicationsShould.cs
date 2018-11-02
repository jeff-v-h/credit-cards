using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace CreditCards.IntegrationTests
{
    public class CreditCardApplicationsShould
    {
        [Fact]
        public async Task RenderApplicationForm()
        {
            // Build a Test Server
            var builder = new WebHostBuilder()
                // path to the CreditCards project
                .UseContentRoot(@"C:\Users\jeff.huang\Documents\education\aspdotnet-core-mvc-testing-fundamentals\testing-fundamentals-project\src\CreditCards")
                .UseEnvironment("Development")
                .UseStartup<CreditCards.Startup>()
                .UseApplicationInsights();

            var server = new TestServer(builder);

            // create the HTTP client
            var client = server.CreateClient();

            var response = await client.GetAsync("/Apply");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("New Credit Card Application", responseString);
        }

        [Fact]
        public async Task NotAcceptPostedApplicationDetailsWithMissingFrequentFlyerNumber()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"C:\Users\jeff.huang\Documents\education\aspdotnet-core-mvc-testing-fundamentals\testing-fundamentals-project\src\CreditCards")
                .UseEnvironment("Development")
                .UseStartup<CreditCards.Startup>()
                .UseApplicationInsights();

            var server = new TestServer(builder);

            var client = server.CreateClient();

            HttpRequestMessage postRequest =
                new HttpRequestMessage(HttpMethod.Post, "/Apply");

            var formData = new Dictionary<string, string>
            {
                {"FirstName", "Sarah"},
                {"LastName", "Smith"},
                {"Age", "18"},
                {"GrossAnnualIncome", "100000"}
                // Frequent flyer number omitted to check we can't proceed with the application
            };

            postRequest.Content = new FormUrlEncodedContent(formData);

            HttpResponseMessage postResponse = await client.SendAsync(postRequest);

            postResponse.EnsureSuccessStatusCode();

            var responseString = await postResponse.Content.ReadAsStringAsync();

            Assert.Contains("Please provide a frequent flyer number", responseString);
        }
    }
}
