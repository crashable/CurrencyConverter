using System.Net.Http.Json;

string baseCurrency = args[0];
string targetCurrency = args[1];
string? date = args.Length > 3 ? args[3] : null;

if (date != null && !DateTime.TryParse(date, out _))
{
	Console.WriteLine("Invalid date. Please provide a valid date in the format YYYY-MM-DD.");
	return;
}



HttpClient httpClient = new HttpClient();
string apiKey = Environment.GetEnvironmentVariable("FIXERIO_API_KEY") ?? throw new InvalidOperationException("API key not found in environment variables.");
string _baseUrl = "https://data.fixer.io/api/";

httpClient.BaseAddress = new Uri(_baseUrl);

if (!decimal.TryParse(args[2], out decimal amount))
{
	Console.WriteLine("Invalid amount. Please provide a valid number for the amount.");
	return;
}

string endpoint = BuildEndpoint(baseCurrency, targetCurrency, date, apiKey);

FixerResponse? fixerResponse = await httpClient.GetFromJsonAsync<FixerResponse>(endpoint);

if (fixerResponse != null && fixerResponse.Success)
{
	ConvertCurrency(fixerResponse, baseCurrency, targetCurrency, amount);
}
else
{
	Console.WriteLine("Failed to retrieve exchange rate data.");
}

string BuildEndpoint(string baseCurrency, string targetCurrency, string? date, string apiKey)
{
	return date != null ? $"{date}?access_key={apiKey}&symbols={baseCurrency},{targetCurrency}" : $"latest?access_key={apiKey}&symbols={baseCurrency},{targetCurrency}";
}

void ConvertCurrency(FixerResponse fixerResponse, string baseCurrency, string targetCurrency, decimal amount)
{
	if (fixerResponse.Rates.TryGetValue(baseCurrency, out decimal baseRate) && fixerResponse.Rates.TryGetValue(targetCurrency, out decimal targetRate))
	{
		decimal amountInEur = amount / baseRate;
		decimal convertedAmount = amountInEur * targetRate;
		Console.WriteLine($"{amount} {baseCurrency} is equal to {convertedAmount} {targetCurrency} on {fixerResponse.Date}");
	}
	else
	{
		Console.WriteLine($"Conversion rates for {baseCurrency} or {targetCurrency} not found.");
	}
}

public class FixerResponse
{
	public bool Success { get; set; }
	public bool Historical { get; set; }
	public long Timestamp { get; set; }
	public string Base { get; set; }
	public string Date { get; set; }
	public Dictionary<string, decimal> Rates { get; set; }
}
