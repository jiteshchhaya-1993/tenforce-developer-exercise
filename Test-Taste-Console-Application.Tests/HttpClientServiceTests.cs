using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Services;
using Xunit;

namespace Test_Taste_Console_Application.Tests
{
    public class HttpClientServiceTests
    {
        [Fact]
        public void HttpClientService_WhenApiKeyProvided_ConfiguresBearerToken()
        {
            var configData = new Dictionary<string, string>
            {
                { ConfigurationFileName.ApiKeyEnvironmentVariable, "test-guid-key-12345" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            using var httpClient = new HttpClient();
            var service = new HttpClientService(httpClient, configuration);

            Assert.NotNull(service.Client.DefaultRequestHeaders.Authorization);
            Assert.Equal("Bearer", service.Client.DefaultRequestHeaders.Authorization.Scheme);
            Assert.Equal("test-guid-key-12345", service.Client.DefaultRequestHeaders.Authorization.Parameter);
        }

        [Fact]
        public void HttpClientService_WhenNoApiKey_DoesNotSetAuthorizationHeader()
        {
            var configuration = new ConfigurationBuilder().Build();

            using var httpClient = new HttpClient();
            var service = new HttpClientService(httpClient, configuration);

            Assert.Null(service.Client.DefaultRequestHeaders.Authorization);
        }
    }
}
