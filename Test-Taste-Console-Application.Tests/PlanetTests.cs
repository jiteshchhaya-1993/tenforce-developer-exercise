using System.Collections.Generic;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Xunit;

namespace Test_Taste_Console_Application.Tests
{
    public class PlanetTests
    {
        [Fact]
        public void HasMoons_WhenMoonsNullOrEmpty_ReturnsFalse()
        {
            var planetWithoutMoons = new Planet(new PlanetDto { Id = "mercury", Moons = null });
            var planetWithEmptyMoons = new Planet(new PlanetDto { Id = "venus", Moons = new List<MoonDto>() });

            Assert.False(planetWithoutMoons.HasMoons());
            Assert.False(planetWithEmptyMoons.HasMoons());
        }

        [Fact]
        public void HasMoons_WhenMoonsExist_ReturnsTrue()
        {
            var planet = new Planet(new PlanetDto
            {
                Id = "earth",
                Moons = new List<MoonDto>
                {
                    new MoonDto { Id = "lune", AvgTemp = 250f }
                }
            });

            Assert.True(planet.HasMoons());
            Assert.Single(planet.Moons);
        }

        [Fact]
        public void AverageMoonTemperature_WhenNoMoons_ReturnsZero()
        {
            var planet = new Planet(new PlanetDto { Id = "venus", Moons = null });
            Assert.Equal(0.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void AverageMoonTemperature_WithMoonsHavingTemperatures_CalculatesCorrectAverage()
        {
            var planet = new Planet(new PlanetDto
            {
                Id = "mars",
                Moons = new List<MoonDto>
                {
                    new MoonDto { Id = "phobos", AvgTemp = 233.0f },
                    new MoonDto { Id = "deimos", AvgTemp = 237.0f }
                }
            });

            // (233 + 237) / 2 = 235.0f
            Assert.Equal(235.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void AverageMoonTemperature_WithMixedKnownAndUnknownTemperatures_AveragesKnownTemperatures()
        {
            var planet = new Planet(new PlanetDto
            {
                Id = "jupiter",
                Moons = new List<MoonDto>
                {
                    new MoonDto { Id = "io", AvgTemp = 130.0f },
                    new MoonDto { Id = "europa", AvgTemp = 102.0f },
                    new MoonDto { Id = "unknown-moon", AvgTemp = null },
                    new MoonDto { Id = "zero-temp-moon", AvgTemp = 0.0f }
                }
            });

            // Known non-zero temperatures: (130 + 102) / 2 = 116.0f
            Assert.Equal(116.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void AverageMoonGravity_WithMoonsHavingGravity_CalculatesCorrectAverage()
        {
            var planet = new Planet(new PlanetDto
            {
                Id = "jupiter",
                Moons = new List<MoonDto>
                {
                    new MoonDto { Id = "io", Gravity = 1.796f },
                    new MoonDto { Id = "europa", Gravity = 1.315f }
                }
            });

            var expected = (1.796f + 1.315f) / 2f;
            Assert.Equal(expected, planet.AverageMoonGravity, 3);
        }
    }
}
