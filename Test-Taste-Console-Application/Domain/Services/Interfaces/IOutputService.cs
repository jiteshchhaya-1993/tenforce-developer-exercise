namespace Test_Taste_Console_Application.Domain.Services.Interfaces
{
    /// <summary>
    /// An output service that can present data from the Solar System OpenData API to the user.
    /// </summary>
    public interface IOutputService
    {
        void OutputAllPlanetsAndTheirMoonsToConsole();
        void OutputAllMoonsAndTheirMassToConsole();
        void OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();
        void OutputAllPlanetsAndTheirAverageMoonGravityToConsole();
    }
}
