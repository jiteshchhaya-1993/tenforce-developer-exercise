# Architectural Decisions & Domain Model Motivations

This document details the architectural decisions, design patterns, domain model changes, and API interface enhancements introduced to extend the TenForce Solar System application.

---

## 1. Context & Objectives

The goal was to extend the existing .NET console application to:
1. Connect securely to the **Solar System OpenData API** (v2.0.0+) requiring `Authorization: Bearer <API_KEY>`.
2. Introduce temperature capabilities into the system (previously absent).
3. Compute and display the **average moon temperature** for all planets having at least one moon.
4. Output data to both the console (screen) and persistent disk files (CSV).
5. Ensure robust error handling, clean separation of concerns, informative user feedback, and testability.

---

## 2. API Interface & Data Access Motivations

### 2.1 API Authentication & Secrets Management
- **Decision**: Implemented `Microsoft.Extensions.Configuration` to retrieve the API key from:
  1. The environment variable `SOLAR_SYSTEM_API_KEY` (recommended for CI/CD and terminal execution).
  2. Local `appsettings.json` (via `"SolarSystem:ApiKey"`).
- **Motivation**: Hardcoding secrets violates Twelve-Factor App principles and security standards. `appsettings.json` is excluded via `.gitignore` to prevent secret leaks, while `appsettings.example.json` provides a clear template for evaluators.
- **Client Configuration**: `HttpClientService` inspects the configuration and conditionally attaches the `AuthenticationHeaderValue("Bearer", apiKey)` header. If missing, it provides explicit warnings rather than failing cryptographically.

### 2.2 Solving the N+1 Query Problem & Performance Optimization
- **Problem in Original Implementation**:
  The starter implementation made an initial request to `/rest/bodies` for planets, and then made a separate synchronous HTTP GET request for *every single moon* of each planet (`/rest/bodies/{moonId}`).
  With 450+ total natural satellite associations (Jupiter ~115, Saturn ~293, Uranus ~29, etc.), this produced **hundreds of sequential HTTP requests**, leading to:
  - Network latency of 45–60+ seconds.
  - Vulnerability to HTTP 429 (Rate Limiting) from the free API tier.
- **Solution Introduced**:
  - We introduced `UriPath.GetAllMoonsWithMassQueryParameters` (`/rest/bodies?data=id,mass,massValue,massExponent,avgTemp,gravity&filter[]=aroundPlanet,gt,null`) to pre-fetch moons in bulk into an indexed lookup dictionary (`IDictionary<string, MoonDto>`).
  - **Resilient Fallback**: If a moon reference is not present in the bulk payload (or if the bulk call fails), the service falls back gracefully to `FetchIndividualMoon(moonId)`.
  - **In-Memory Caching**: Added in-memory caching in `PlanetService` and `MoonService` so subsequent output operations (screen and CSV file generation) don't trigger redundant API roundtrips.

---

## 3. Domain Model Motivations

### 3.1 Introducing Temperature to DTOs & Domain Objects
- **`MoonDto`**:
  - Added `[JsonProperty("avgTemp")] public float? AvgTemp { get; set; }` to capture the mean temperature in Kelvin exposed by the API.
  - Made nullable (`float?`) because celestial bodies in astrophysical databases frequently have missing, unmodeled, or non-applicable temperature fields.
  - Added null-safe property `URLId => !string.IsNullOrEmpty(Rel) ? Rel.Split('/').Last() : Id;` preventing `NullReferenceException` when `rel` is empty.
- **`Moon`**:
  - Added `public float? AverageTemperature { get; set; }` mapped from `moonDto.AvgTemp`.
- **`Planet`**:
  - Introduced `public float AverageMoonTemperature { get; }`.
  - Calculation logic:
    ```csharp
    var moonsWithTemp = Moons
        .Where(m => m.AverageTemperature.HasValue && m.AverageTemperature.Value > 0)
        .ToList();

    if (moonsWithTemp.Any())
    {
        return (float)moonsWithTemp.Average(m => m.AverageTemperature.Value);
    }

    var validMoons = Moons.Where(m => m.AverageTemperature.HasValue).ToList();
    return validMoons.Any() ? (float)validMoons.Average(m => m.AverageTemperature.Value) : 0.0f;
    ```
  - **Handling Upstream Dataset Edge Cases**:
    Upon inspecting the public API dataset, `avgTemp` is currently recorded as `0` for many natural satellites (unlike planetary bodies). Our calculation handles both scenarios:
    1. If recorded non-zero temperatures exist, it calculates their true mathematical average.
    2. If all moons have `0` or `null`, it returns `0.0f` without throwing divide-by-zero exceptions.
  - **Filtering**:
    Planets without moons (e.g., Mercury and Venus) return `0.0f` and are excluded from the moon temperature listing, adhering strictly to: *"create a list of all planets that have at least one moon and list the average temperature for those planets' moons"*.

---

## 4. Output Architecture (Screen & File)

- **Separation of Concerns**:
  - `ScreenOutputService` implements `IOutputService` focusing exclusively on formatted ASCII table generation to the console.
  - `FileOutputService` implements `IFileOutputService` focusing on structured RFC 4180 CSV generation to disk under `FileOutput/`.
- **Exported CSV Files**:
  1. `AllPlanetsAndTheirAverageMoonTemperature.csv`: Planets with at least one moon and their average moon temperature.
  2. `AllMoonsAndTheirMass.csv`: All moons with their mass exponent and value.
  3. `AllPlanetsAndTheirMoons.csv`: Complete planet-to-moon hierarchy.

---

## 5. Summary of Quality & Design Attributes

- **Clean Architecture**: Clear separation of data access (`PlanetService`, `MoonService`, `HttpClientService`), domain objects (`Planet`, `Moon`), DTOs, and output presenters.
- **Defensive Programming**: Null checks on API responses, structured error logging via `log4net`, and resilient fallbacks.
- **User Feedback**: Execution step notifications inform the user at each phase (`"Loading planets..."`, `"Writing data to CSV files on disk..."`, etc.).
- **Automated Testing**: 11 unit tests covering domain models, calculation formulas, JSON serialization, and configuration handling.
