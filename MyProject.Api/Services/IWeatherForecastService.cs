using MyProject.Api.Models;

namespace MyProject.Api.Services;

/// <summary>
/// Defines operations for weather forecast data
/// </summary>
public interface IWeatherForecastService
{
    /// <summary>
    /// Gets weather forecasts for the specified number of days
    /// </summary>
    /// <param name="days">The number of days to forecast</param>
    /// <returns>A collection of weather forecasts</returns>
    IEnumerable<WeatherForecast> GetForecasts(int days = 5);
} 