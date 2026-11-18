namespace Test_Taste_Console_Application.Domain.Services.Interfaces
{
    /// <summary>
    /// Service to export Solar System data to disk files (CSV).
    /// </summary>
    public interface IFileOutputService
    {
        void OutputAllPlanetsAndTheirMoonsToFile();
        void OutputAllMoonsAndTheirMassToFile();
        void OutputAllPlanetsAndTheirAverageMoonTemperatureToFile();
    }
}
