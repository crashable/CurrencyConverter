using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ConversionController : ControllerBase
	{
		private readonly ILogger<ConversionController> _logger;
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _httpClientFactory;


		public ConversionController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ConversionController> logger)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount, [FromQuery] string? date)
		{
			// Retrieve the API key from configuration/environment variables.
			string? apiKey = _configuration["FIXERIO_API_KEY"];
			if (string.IsNullOrEmpty(apiKey))
			{
				return Problem("API key is not configured.");
			}

			// Build the API endpoint. If a date is provided, use it; otherwise use 'latest'
			string endpoint = date != null
				? $"{date}?access_key={apiKey}&symbols={from},{to}"
				: $"latest?access_key={apiKey}&symbols={from},{to}";

			var client = _httpClientFactory.CreateClient();
			var fixerResponse = await client.GetFromJsonAsync<FixerResponse>($"https://data.fixer.io/api/{endpoint}");

			if (fixerResponse == null || !fixerResponse.Success)
			{
				return Problem("Failed to retrieve exchange rate data.");
			}

			if (!fixerResponse.Rates.TryGetValue(from, out decimal baseRate) ||
				!fixerResponse.Rates.TryGetValue(to, out decimal targetRate))
			{
				return Problem("Conversion rates not available for the provided currencies.");
			}

			// Convert using EUR as intermediary (since the free plan uses EUR as the base)
			decimal amountInEur = amount / baseRate;
			decimal convertedAmount = amountInEur * targetRate;

			// Return the conversion result as JSON.
			return Ok(new
			{
				From = from,
				To = to,
				Amount = amount,
				ConvertedAmount = convertedAmount,
				Date = fixerResponse.Date
			});
		}
	}
	public class FixerResponse
	{
		public bool Success { get; set; }
		public string Base { get; set; }
		public string Date { get; set; }
		public Dictionary<string, decimal> Rates { get; set; }
	}
}
