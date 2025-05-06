using Microsoft.AspNetCore.Mvc;
using MyProject.Api.Models;
using MyProject.Api.Services;

namespace MyProject.Api.Controllers;

/// <summary>
/// API controller for weather forecast data
/// </summary>
[ApiController]
[Route("api/weatherforecast")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<WeatherForecastController> _logger;

    /// <summary>
    /// Initializes a new instance of the WeatherForecastController class
    /// </summary>
    /// <param name="weatherForecastService">The weather forecast service</param>
    /// <param name="logger">The logger</param>
    public WeatherForecastController(
        IWeatherForecastService weatherForecastService,
        ILogger<WeatherForecastController> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    /// <summary>
    /// Gets weather forecast data
    /// </summary>
    /// <param name="days">Optional number of days to forecast</param>
    /// <returns>A collection of weather forecasts</returns>
    [HttpGet]
    public ActionResult<IEnumerable<WeatherForecast>> Get([FromQuery] int days = 5)
    {
        try
        {
            if (days <= 0 || days > 14)
            {
                return BadRequest("The number of forecast days must be between 1 and 14.");
            }

            var forecast = _weatherForecastService.GetForecasts(days);
            return Ok(forecast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather forecast");
            return StatusCode(500, "An error occurred while retrieving the weather forecast");
        }
    }
} 