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
    }
}
