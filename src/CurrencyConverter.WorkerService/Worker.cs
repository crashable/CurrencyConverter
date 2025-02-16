using CurrencyConverter.WorkerService.Data;
using CurrencyConverter.WorkerService.Models;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace CurrencyConverter.WorkerService
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IConfiguration _configuration;
		private bool hasRunOnce = false;

		public Worker(ILogger<Worker> logger,IConfiguration configuration, IHttpClientFactory httpClientFactory, IServiceScopeFactory serviceScopeFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_serviceScopeFactory = serviceScopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
				{
					_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
				}
				await Task.Delay(1000, stoppingToken);
				if (!hasRunOnce) {
					try
					{
						string? apiKey = _configuration["FIXERIO_API_KEY"];
						var httpClient = _httpClientFactory.CreateClient();
						string symbolsEndpoint = $"https://data.fixer.io/api/symbols?access_key={apiKey}";


						var symbolsResponse = await httpClient.GetFromJsonAsync<SymbolsResponse>(symbolsEndpoint);


						string symbolKeys = string.Join(",", symbolsResponse.Symbols.Keys);
						string latestRatesEndpoint = $"https://data.fixer.io/api/latest?access_key={apiKey}&symbols={symbolKeys}";
						_logger.LogInformation("symbols: {symbolKeys}", symbolKeys);




						var exchangeRatesResponse = await httpClient.GetFromJsonAsync<LatestRatesResponse>(latestRatesEndpoint);

						if (exchangeRatesResponse != null && exchangeRatesResponse.Success)
						{
							_logger.LogInformation("Exchange rates retrieved successfully.");

							using (var scope = _serviceScopeFactory.CreateScope())
							{
								var dbContext = scope.ServiceProvider.GetRequiredService<ExchangeRateContext>();

								var exchangeRateSnapshot = new ExchangeRateSnapshot
								{
									SnapshotDate = DateTime.Parse(exchangeRatesResponse.Date),
									BaseCurrency = exchangeRatesResponse.Base,
									Timestamp = exchangeRatesResponse.Timestamp,
									Rates = exchangeRatesResponse.Rates.Select(rate => new ExchangeRate
									{
										CurrencyCode = rate.Key,
										Rate = rate.Value
									}).ToList()
								};
								dbContext.Snapshots.Add(exchangeRateSnapshot);
								await dbContext.SaveChangesAsync(stoppingToken);
								_logger.LogInformation("Exchange rates saved to database.");

							}
						}
					}
					catch (Exception ex) { _logger.LogError(ex, "An error occurred."); }
					hasRunOnce = true;

				}

			}
		}
	}

	public class SymbolsResponse
	{
		public bool Success { get; set; }
		public Dictionary<string, string> Symbols { get; set; }
	}

	public class LatestRatesResponse
	{
		public bool Success { get; set; }
		public long Timestamp { get; set; }
		public string Base { get; set; }
		public string Date { get; set; }
		public Dictionary<string, decimal> Rates { get; set; }
	}
}
