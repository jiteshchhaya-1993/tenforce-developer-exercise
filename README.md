# TenForce – Developer Exercise: Test and Taste

A .NET console application that retrieves data from the [Solar System OpenData API](https://api.le-systeme-solaire.net/), processes planetary and lunar data, calculates average lunar temperatures per planet, and outputs the results to both the console and CSV files on disk.

---

## Features

- **Solar System API Integration (v2.0.0+)**: Authenticated HTTP client using Bearer Token authentication.
- **Configurable Secrets**: Flexible configuration via environment variables or `appsettings.json` (excluded from git).
- **Domain Modeling**:
  - `Planet`: Models planetary data and calculates `AverageMoonTemperature` for planets that have moons.
  - `Moon`: Models lunar properties including `MassValue`, `MassExponent`, and `AverageTemperature`.
  - `PlanetDto` & `MoonDto`: Deserialization mapping with JSON path conversion.
- **Dual Output Handlers**:
  - **Screen Output**: Beautifully formatted tables rendered to the terminal.
  - **File Output (CSV)**: Automated generation of structured CSV files in `FileOutput/`.
- **Performance Optimization**: Bulk pre-fetching of moons eliminating the N+1 API query problem, complemented by in-memory caching.
- **Unit Test Suite**: 11 automated unit tests with xUnit covering calculation edge cases, DTO parsing, and configuration loading.

---

## Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or [.NET 5.0 SDK]
- Solar System OpenData API Key (free from [api.le-systeme-solaire.net/generatekey.html](https://api.le-systeme-solaire.net/generatekey.html))

### Setting the API Key

You can configure the API key in one of two ways:

#### Option A: Environment Variable (Recommended)
**PowerShell:**
```powershell
$env:SOLAR_SYSTEM_API_KEY="YOUR_API_KEY_GUID"
```

**Bash / Linux / macOS:**
```bash
export SOLAR_SYSTEM_API_KEY="YOUR_API_KEY_GUID"
```

**Windows CMD:**
```cmd
set SOLAR_SYSTEM_API_KEY=YOUR_API_KEY_GUID
```

#### Option B: `appsettings.json`
Create or edit `Test-Taste-Console-Application/appsettings.json` (a template is provided in `appsettings.example.json`):
```json
{
  "SolarSystem": {
    "ApiKey": "YOUR_API_KEY_GUID"
  }
}
```
*(Note: `appsettings.json` is listed in `.gitignore` to prevent committing API secrets).*

---

## Running the Application

From the repository root or project directory:

```bash
cd Test-Taste-Console-Application
dotnet run
```

### Generated CSV Output Files
The application automatically creates the `FileOutput/` folder and produces:
- `AllPlanetsAndTheirAverageMoonTemperature.csv` – All planets that have at least one moon, with their average moon temperature (K).
- `AllMoonsAndTheirMass.csv` – All moons and their mass exponent/value.
- `AllPlanetsAndTheirMoons.csv` – Full hierarchical list of planets and their moons.

---

## Running Unit Tests

To run the xUnit test suite:

```bash
dotnet test
```

---

## Architecture & Design Decisions

For a detailed explanation of domain model modifications, API interface design, and performance optimizations, please read [ARCHITECTURAL_DECISIONS.md](ARCHITECTURAL_DECISIONS.md).

---

## Git Branching Strategy

- `main` – Production-ready, stable codebase.
- `TenForce-JC` – Feature branch holding intermediate commits documenting step-by-step progress.
