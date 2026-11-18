using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <summary>
    /// A service to configure and provide the HttpClient for interacting with the Solar System OpenData API.
    /// Supports Bearer token authentication required by API v2.0.0+.
    /// </summary>
    public class HttpClientService
    {
        public HttpClient Client { get; }

        public HttpClientService(HttpClient client, IConfiguration configuration = null)
        {
            Client = client;
            Client.BaseAddress = new Uri(UriPath.BaseUri);
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(HttpClientSettings.JsonType));

            // Retrieve API key from environment variable or appsettings.json
            var apiKey = configuration?[ConfigurationFileName.ApiKeyEnvironmentVariable]
                         ?? configuration?["SolarSystem:ApiKey"]
                         ?? Environment.GetEnvironmentVariable(ConfigurationFileName.ApiKeyEnvironmentVariable);

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey.Trim());
            }
            else
            {
                Logger.Instance.Warn("Solar System API key is not configured. Requests may fail with HTTP 401 (Unauthorized).");
            }
        }
    }
}
