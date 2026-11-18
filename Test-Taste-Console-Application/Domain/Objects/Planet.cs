using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class Planet
    {
        public string Id { get; set; }
        public float SemiMajorAxis { get; set; }
        public ICollection<Moon> Moons { get; set; }

        /// <summary>
        /// Calculates the average temperature of the planet's moons (in Kelvin).
        /// Returns 0.0f if the planet has no moons or no temperature measurements.
        /// </summary>
        public float AverageMoonTemperature
        {
            get
            {
                if (!HasMoons())
                {
                    return 0.0f;
                }

                // Consider moons that have recorded temperature data
                var moonsWithTemp = Moons
                    .Where(m => m.AverageTemperature.HasValue && m.AverageTemperature.Value > 0)
                    .ToList();

                if (moonsWithTemp.Any())
                {
                    return (float)moonsWithTemp.Average(m => m.AverageTemperature.Value);
                }

                // If all moons report 0/null in the API catalog, return average across moons with values, or 0.0f
                var validMoons = Moons.Where(m => m.AverageTemperature.HasValue).ToList();
                return validMoons.Any() ? (float)validMoons.Average(m => m.AverageTemperature.Value) : 0.0f;
            }
        }

        /// <summary>
        /// Calculates the average surface gravity of the planet's moons (in m/s^2).
        /// </summary>
        public float AverageMoonGravity
        {
            get
            {
                if (!HasMoons())
                {
                    return 0.0f;
                }

                var moonsWithGravity = Moons
                    .Where(m => m.Gravity.HasValue && m.Gravity.Value > 0)
                    .ToList();

                return moonsWithGravity.Any()
                    ? (float)moonsWithGravity.Average(m => m.Gravity.Value)
                    : 0.0f;
            }
        }

        public Planet(PlanetDto planetDto)
        {
            Id = planetDto.Id;
            SemiMajorAxis = planetDto.SemiMajorAxis;
            Moons = new Collection<Moon>();
            if (planetDto.Moons != null)
            {
                foreach (MoonDto moonDto in planetDto.Moons)
                {
                    Moons.Add(new Moon(moonDto));
                }
            }
        }

        public bool HasMoons()
        {
            return Moons != null && Moons.Count > 0;
        }
    }
}
