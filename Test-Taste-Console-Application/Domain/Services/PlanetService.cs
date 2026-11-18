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
    public class PlanetService : IPlanetService
    {
        private readonly HttpClientService _httpClientService;
        private IEnumerable<Planet> _cachedPlanets;

        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Planet> GetAllPlanets()
        {
            if (_cachedPlanets != null)
            {
                return _cachedPlanets;
            }

            var allPlanetsWithTheirMoons = new Collection<Planet>();

            try
            {
                Console.WriteLine(OutputString.LoadingPlanets);

                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                    .Result;

                // If the status code isn't 200-299, log warning and return empty collection
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    Console.WriteLine($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    return allPlanetsWithTheirMoons;
                }

                var content = response.Content.ReadAsStringAsync().Result;
                var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);

                if (results?.Bodies == null) return allPlanetsWithTheirMoons;

                // Performance Optimization:
                // Pre-fetch all moons in a single query to eliminate the N+1 query problem
                // (reducing ~280 sequential HTTP calls down to a single request).
                var moonsDictionary = FetchMoonsDictionary();

                foreach (var planet in results.Bodies)
                {
                    if (planet.Moons != null && planet.Moons.Any())
                    {
                        var newMoonsCollection = new Collection<MoonDto>();
                        foreach (var moonRef in planet.Moons)
                        {
                            var moonId = moonRef.URLId;
                            if (moonsDictionary != null && moonsDictionary.TryGetValue(moonId, out var cachedMoon))
                            {
                                newMoonsCollection.Add(cachedMoon);
                            }
                            else
                            {
                                // Resilient Fallback: Fetch individual moon by ID if not found in bulk response
                                var individualMoon = FetchIndividualMoon(moonId);
                                if (individualMoon != null)
                                {
                                    newMoonsCollection.Add(individualMoon);
                                }
                            }
                        }
                        planet.Moons = newMoonsCollection;
                    }

                    allPlanetsWithTheirMoons.Add(new Planet(planet));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error occurred while fetching planets: {ex.Message}");
                Console.WriteLine($"Error occurred while fetching planets: {ex.Message}");
            }

            if (allPlanetsWithTheirMoons.Any())
            {
                _cachedPlanets = allPlanetsWithTheirMoons;
            }

            return allPlanetsWithTheirMoons;
        }

        private IDictionary<string, MoonDto> FetchMoonsDictionary()
        {
            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllMoonsWithMassQueryParameters)
                    .Result;

                if (!response.IsSuccessStatusCode) return null;

                var content = response.Content.ReadAsStringAsync().Result;
                var results = JsonConvert.DeserializeObject<JsonResult<MoonDto>>(content);

                return results?.Bodies?
                    .GroupBy(m => m.Id, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn($"Bulk moon pre-fetch failed, will fallback to individual queries: {ex.Message}");
                return null;
            }
        }

        private MoonDto FetchIndividualMoon(string moonId)
        {
            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetMoonByIdQueryParameters + moonId)
                    .Result;

                if (!response.IsSuccessStatusCode) return null;

                var content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<MoonDto>(content);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Failed to fetch individual moon '{moonId}': {ex.Message}");
                return null;
            }
        }
    }
}
