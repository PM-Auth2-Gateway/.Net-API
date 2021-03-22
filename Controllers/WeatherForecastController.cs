using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Eto nam ne nado, prosto testoviy endpoint
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// WeatherForecastController constructor
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Daje ne otkrivai
        /// </summary>
        /// <returns>IEnumerable WeatherForecast</returns>
        /// <remarks>
        /// Ny i zachem ti syda zawel???
        /// </remarks>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogWarning("Request path: " + Request.Path + ", pathBase: "
                + Request.PathBase + ", host: value => " + Request.Host.Value + 
                ", host.host => " + Request.Host.Host + ", scheme: " + Request.Scheme + ", localIpAdress"
                + Request.HttpContext.Connection.LocalIpAddress);

            _logger.LogInformation("Request path: " + Request.Path + ", pathBase: "
                + Request.PathBase + ", host: value => " + Request.Host.Value +
                ", host.host => " + Request.Host.Host + ", scheme: " + Request.Scheme + ", localIpAdress"
                + Request.HttpContext.Connection.LocalIpAddress);
            return Redirect("pmacademy://");
        }
    }
}
