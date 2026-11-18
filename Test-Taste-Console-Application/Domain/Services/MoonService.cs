using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class MoonService : IMoonService
    {
        private readonly HttpClientService _httpClientService;
        private IEnumerable<Moon> _cachedMoons;

        public MoonService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Moon> GetAllMoons()
        {
            if (_cachedMoons != null)
            {
                return _cachedMoons;
            }

            var allMoons = new Collection<Moon>();

            try
            {
                Console.WriteLine(OutputString.LoadingMoons);

                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllMoonsWithMassQueryParameters)
                    .Result;

                // If the status code isn't 200-299, return an empty collection
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    Console.WriteLine($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    return allMoons;
                }

                var content = response.Content.ReadAsStringAsync().Result;
                var results = JsonConvert.DeserializeObject<JsonResult<MoonDto>>(content);

                if (results?.Bodies == null) return allMoons;

                foreach (var moonDto in results.Bodies)
                {
                    allMoons.Add(new Moon(moonDto));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error occurred while fetching moons: {ex.Message}");
                Console.WriteLine($"Error occurred while fetching moons: {ex.Message}");
            }

            if (allMoons.Any())
            {
                _cachedMoons = allMoons;
            }

            return allMoons;
        }
    }
}
