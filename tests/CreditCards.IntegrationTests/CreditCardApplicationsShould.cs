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
        private const string AntiForgeryFieldName = "__AFTField";
        private const string AntiForgeryCookieName = "AFTCookie";

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
                .ConfigureServices(x =>
                {
                    x.AddAntiforgery(t =>
                    {
                        t.CookieName = AntiForgeryCookieName;
                        t.FormFieldName = AntiForgeryFieldName;
                    });
                })
                .UseApplicationInsights();

            var server = new TestServer(builder);
            var client = server.CreateClient();

            // Get initial response that contains anti forgery tokens
            HttpResponseMessage initialResponse = await client.GetAsync("/Apply");

            string antiForgeryCookieValue = ExtractAntiForgeryCookieValueFrom(initialResponse);
            string antiForgeryToken = ExtractAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());


            // Create POST request, adding anti forgery cookie and form field
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/Apply");

            postRequest.Headers.Add("Cookie",
                new CookieHeaderValue(AntiForgeryCookieName, antiForgeryCookieValue).ToString());

            var formData = new Dictionary<string, string>
            {
                {AntiForgeryFieldName, antiForgeryToken},
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

        private static string ExtractAntiForgeryCookieValueFrom(
            HttpResponseMessage response)
        {
            string antiForgeryCookie = response.Headers.GetValues("Set-Cookie")
                .FirstOrDefault(x => x.Contains(AntiForgeryCookieName));

            if (antiForgeryCookie is null)
            {
                throw new ArgumentException(
                    $"Cookie '{AntiForgeryCookieName}' not found in HTTP response",
                    nameof(response));
            }

            string antiForgeryCookieValue =
                SetCookieHeaderValue.Parse(antiForgeryCookie).Value;

            return antiForgeryCookieValue;
        }

        private string ExtractAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch =
                Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if (requestVerificationTokenMatch.Success)
            {
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' not found in HTML", nameof(htmlBody));
        }
    }
}
