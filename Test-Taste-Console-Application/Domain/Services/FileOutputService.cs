using System;
using System.IO;
using System.Linq;
using System.Text;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class FileOutputService : IFileOutputService
    {
        private readonly IPlanetService _planetService;
        private readonly IMoonService _moonService;

        public FileOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        private void EnsureOutputDirectoryExists()
        {
            if (!Directory.Exists(PathName.PathToOutputFolder))
            {
                Directory.CreateDirectory(PathName.PathToOutputFolder);
                Console.WriteLine($"{OutputString.FolderCreated}{PathName.PathToOutputFolder}");
                Logger.Instance.Info($"{OutputString.FolderCreated}{PathName.PathToOutputFolder}");
            }
        }

        public void OutputAllPlanetsAndTheirMoonsToFile()
        {
            try
            {
                EnsureOutputDirectoryExists();
                var planets = _planetService.GetAllPlanets().ToArray();

                if (!planets.Any())
                {
                    Logger.Instance.Warn("Cannot write planets to file: No planets found.");
                    return;
                }

                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirMoonsFile);
                var sb = new StringBuilder();
                sb.AppendLine("Planet's Number,Planet's Id,Planet's Semi-Major Axis,Total Moons,Moon's Number,Moon's Id");

                for (int i = 0, planetNum = 1; i < planets.Length; i++, planetNum++)
                {
                    var planet = planets[i];
                    var planetId = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);

                    if (planet.HasMoons())
                    {
                        for (int k = 0, moonNum = 1; k < planet.Moons.Count; k++, moonNum++)
                        {
                            var moon = planet.Moons.ElementAt(k);
                            var moonId = CultureInfoUtility.TextInfo.ToTitleCase(moon.Id);
                            sb.AppendLine($"{planetNum},{planetId},{planet.SemiMajorAxis},{planet.Moons.Count},{moonNum},{moonId}");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"{planetNum},{planetId},{planet.SemiMajorAxis},{planet.Moons.Count},-,-");
                    }
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Logger.Instance.Info($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        public void OutputAllMoonsAndTheirMassToFile()
        {
            try
            {
                EnsureOutputDirectoryExists();
                var moons = _moonService.GetAllMoons().ToArray();

                if (!moons.Any())
                {
                    Logger.Instance.Warn("Cannot write moons to file: No moons found.");
                    return;
                }

                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllMoonsAndTheirMassFile);
                var sb = new StringBuilder();
                sb.AppendLine("Moon's Number,Moon's Id,Moon's Mass Exponent,Moon's Mass Value");

                for (int i = 0, j = 1; i < moons.Length; i++, j++)
                {
                    var moon = moons[i];
                    var moonId = CultureInfoUtility.TextInfo.ToTitleCase(moon.Id);
                    sb.AppendLine($"{j},{moonId},{moon.MassExponent},{moon.MassValue}");
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Logger.Instance.Info($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        /// <summary>
        /// Writes all planets that have at least one moon and their moons' average temperature (in Kelvin) to CSV.
        /// </summary>
        public void OutputAllPlanetsAndTheirAverageMoonTemperatureToFile()
        {
            try
            {
                EnsureOutputDirectoryExists();
                var planets = _planetService.GetAllPlanets()
                    .Where(p => p.HasMoons())
                    .ToArray();

                if (!planets.Any())
                {
                    Logger.Instance.Warn("Cannot write planets average temperature to file: No planets with moons found.");
                    return;
                }

                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirAverageMoonTemperatureFile);
                var sb = new StringBuilder();
                sb.AppendLine("Planet's Number,Planet's Id,Total Moons,Average Moon Temperature (K)");

                for (int i = 0, j = 1; i < planets.Length; i++, j++)
                {
                    var planet = planets[i];
                    var planetId = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);
                    sb.AppendLine($"{j},{planetId},{planet.Moons.Count},{planet.AverageMoonTemperature:F2}");
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Logger.Instance.Info($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }
    }
}
