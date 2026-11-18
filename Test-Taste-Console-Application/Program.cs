using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Services;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            // Configure services into DI container
            ConfigureServices(serviceCollection);

            // Execute service operations to generate screen and file outputs
            RunServiceOperations(serviceCollection);
        }

        private static void RunServiceOperations(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var screenOutputService = serviceProvider.GetService<IOutputService>();
            var fileOutputService = serviceProvider.GetService<IFileOutputService>();

            try
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine("                  TENFORCE SOLAR SYSTEM DATA PROCESSOR                          ");
                Console.WriteLine("================================================================================");
                Console.WriteLine();

                // 1. Output to Console (Screen)
                Console.WriteLine(OutputString.WritingScreenOutput);
                Console.WriteLine();

                // Planets with moons and their average moon temperature
                screenOutputService.OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();

                // Moons and their mass
                screenOutputService.OutputAllMoonsAndTheirMassToConsole();

                // Planets and their moons
                screenOutputService.OutputAllPlanetsAndTheirMoonsToConsole();

                // 2. Output to Disk (CSV Files)
                Console.WriteLine(OutputString.WritingFileOutput);
                fileOutputService.OutputAllPlanetsAndTheirAverageMoonTemperatureToFile();
                fileOutputService.OutputAllMoonsAndTheirMassToFile();
                fileOutputService.OutputAllPlanetsAndTheirMoonsToFile();

                Console.WriteLine();
                Console.WriteLine(OutputString.OperationsCompleted);
            }
            catch (Exception exception)
            {
                Logger.Instance.Error($"{LoggerMessage.ScreenOutputOperationFailed}{exception.Message}");
                Console.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
                System.Diagnostics.Debug.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
            }

            serviceProvider.Dispose();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Configure Log4Net logging
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()),
                new FileInfo(ConfigurationFileName.Logger));

            // Configure appsettings and environment variables configuration
            var configBuilder = new ConfigurationBuilder();

            // Support finding appsettings.json both in current working directory and in the binary base directory
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFileName.AppSettings)))
            {
                configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            }
            else
            {
                configBuilder.SetBasePath(AppContext.BaseDirectory);
            }

            var configuration = configBuilder
                .AddJsonFile(ConfigurationFileName.AppSettings, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddHttpClient<HttpClientService>();
            serviceCollection.AddSingleton<IPlanetService, PlanetService>();
            serviceCollection.AddSingleton<IMoonService, MoonService>();
            serviceCollection.AddSingleton<IOutputService, ScreenOutputService>();
            serviceCollection.AddSingleton<IFileOutputService, FileOutputService>();
        }
    }
}
