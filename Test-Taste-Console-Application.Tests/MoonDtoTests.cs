using Newtonsoft.Json;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Xunit;

namespace Test_Taste_Console_Application.Tests
{
    public class MoonDtoTests
    {
        [Fact]
        public void MoonDto_JsonDeserialization_PopulatesAllProperties()
        {
            var json = @"{
                ""id"": ""lune"",
                ""moon"": ""La Lune"",
                ""rel"": ""https://api.le-systeme-solaire.net/rest/bodies/lune"",
                ""mass"": {
                    ""massValue"": 7.346,
                    ""massExponent"": 22
                },
                ""avgTemp"": 250,
                ""gravity"": 1.62
            }";

            var moonDto = JsonConvert.DeserializeObject<MoonDto>(json);

            Assert.NotNull(moonDto);
            Assert.Equal("La Lune", moonDto.Moon);
            Assert.Equal("lune", moonDto.URLId);
            Assert.Equal(7.346f, moonDto.MassValue, 3);
            Assert.Equal(22f, moonDto.MassExponent);
            Assert.Equal(250f, moonDto.AvgTemp);
            Assert.Equal(1.62f, moonDto.Gravity);
        }

        [Fact]
        public void MoonDto_URLId_WhenRelIsNull_FallsBackToId()
        {
            var moonDto = new MoonDto { Id = "phobos", Rel = null };
            Assert.Equal("phobos", moonDto.URLId);
        }

        [Fact]
        public void MoonDto_URLId_ExtractsLastSegmentOfRel()
        {
            var moonDto = new MoonDto { Rel = "https://api.le-systeme-solaire.net/rest/bodies/titan" };
            Assert.Equal("titan", moonDto.URLId);
        }
    }
}
