using MyProject.Api.Models;
using MyProject.Api.Services;
using Xunit;

namespace MyProject.Api.UnitTests.Services
{
    public class WeatherForecastServiceTests
    {
        private readonly WeatherForecastService _service;

        public WeatherForecastServiceTests()
        {
            _service = new WeatherForecastService();
        }

        [Fact]
        public void GetForecasts_WithDefaultDays_ShouldReturnFiveForecasts()
        {
            // Act
            var result = _service.GetForecasts();

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void GetForecasts_WithSpecificDays_ShouldReturnCorrectNumberOfForecasts()
        {
            // Arrange
            int days = 3;

            // Act
            var result = _service.GetForecasts(days);

            // Assert
            Assert.Equal(days, result.Count());
        }

        [Fact]
        public void GetForecasts_ShouldHaveValidTemperatures()
        {
            // Act
            var result = _service.GetForecasts().ToList();

            // Assert
            foreach (var forecast in result)
            {
                Assert.InRange(forecast.TemperatureC, -20, 55);
                Assert.NotEqual(0, forecast.TemperatureF);
            }
        }

        [Fact]
        public void GetForecasts_ShouldHaveValidDates()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Act
            var result = _service.GetForecasts().ToList();

            // Assert
            for (int i = 0; i < result.Count; i++)
            {
                // Dates should be sequential starting from tomorrow
                Assert.Equal(today.AddDays(i + 1), result[i].Date);
            }
        }

        [Fact]
        public void GetForecasts_ShouldCalculateFahrenheitCorrectly()
        {
            // Act
            var result = _service.GetForecasts().ToList();

            // Assert
            foreach (var forecast in result)
            {
                // F = 32 + (C / 0.5556)
                int expectedF = 32 + (int)(forecast.TemperatureC / 0.5556);
                Assert.Equal(expectedF, forecast.TemperatureF);
            }
        }
    }
} 