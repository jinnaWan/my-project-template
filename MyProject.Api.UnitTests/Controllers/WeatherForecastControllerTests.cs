using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Controllers;
using MyProject.Api.Models;
using MyProject.Api.Services;
using Xunit;

namespace MyProject.Api.UnitTests.Controllers
{
    public class WeatherForecastControllerTests
    {
        private readonly Mock<IWeatherForecastService> _mockWeatherService;
        private readonly Mock<ILogger<WeatherForecastController>> _mockLogger;
        private readonly WeatherForecastController _controller;

        public WeatherForecastControllerTests()
        {
            _mockWeatherService = new Mock<IWeatherForecastService>();
            _mockLogger = new Mock<ILogger<WeatherForecastController>>();
            _controller = new WeatherForecastController(_mockWeatherService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Get_WithDefaultDays_ReturnsOkResult()
        {
            // Arrange
            var forecasts = new List<WeatherForecast>
            {
                new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 20, Summary = "Mild" },
                new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = 25, Summary = "Warm" }
            };
            
            _mockWeatherService.Setup(service => service.GetForecasts(5))
                .Returns(forecasts);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedForecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
            Assert.Equal(2, returnedForecasts.Count());
        }

        [Fact]
        public void Get_WithSpecificDays_ReturnsOkResult()
        {
            // Arrange
            var forecasts = new List<WeatherForecast>
            {
                new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 20, Summary = "Mild" },
                new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = 25, Summary = "Warm" },
                new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), TemperatureC = 30, Summary = "Hot" }
            };
            
            _mockWeatherService.Setup(service => service.GetForecasts(3))
                .Returns(forecasts);

            // Act
            var result = _controller.Get(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedForecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
            Assert.Equal(3, returnedForecasts.Count());
        }

        [Fact]
        public void Get_WithInvalidDays_ReturnsBadRequest()
        {
            // Arrange - negative days
            // Act
            var resultNegative = _controller.Get(-1);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(resultNegative.Result);
            
            // Arrange - too many days
            // Act
            var resultTooMany = _controller.Get(15);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(resultTooMany.Result);
        }

        [Fact]
        public void Get_WhenExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            _mockWeatherService.Setup(service => service.GetForecasts(It.IsAny<int>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = _controller.Get();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
} 