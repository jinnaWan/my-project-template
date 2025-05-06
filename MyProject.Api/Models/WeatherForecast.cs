namespace MyProject.Api.Models;

/// <summary>
/// Represents weather forecast data
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// Gets or sets the date of the forecast
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// Gets or sets the temperature in Celsius
    /// </summary>
    public int TemperatureC { get; set; }
    
    /// <summary>
    /// Gets or sets a text summary of the weather
    /// </summary>
    public string? Summary { get; set; }
    
    /// <summary>
    /// Gets the temperature in Fahrenheit, calculated from TemperatureC
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
} 