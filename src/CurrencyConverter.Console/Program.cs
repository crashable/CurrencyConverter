using System.Text.Json;

string baseCurrency = args[0];
string targetCurrency = args[1];
string? date = args.Length > 3 ? args[3] : null;

HttpClient httpClient = new HttpClient();
string apiKey = Environment.GetEnvironmentVariable("FIXERIO_API_KEY") ?? throw new InvalidOperationException("API key not found in environment variables.");
string _baseUrl = "https://data.fixer.io/api/";

httpClient.BaseAddress = new Uri(_baseUrl);

if (!decimal.TryParse(args[2], out decimal amount))
{
	Console.WriteLine("Invalid amount. Please provide a valid number for the amount.");
	return;
}

// Checking whether a date is provided and set the endpoint accordingly
string endpoint = date != null ? $"{date}?access_key={apiKey}&symbols={baseCurrency},{targetCurrency}" : $"latest?access_key={apiKey}&symbols={baseCurrency},{targetCurrency}";

HttpResponseMessage response = await httpClient.GetAsync(endpoint);
response.EnsureSuccessStatusCode();

string responseContent = await response.Content.ReadAsStringAsync();

FixerResponse? fixerResponse = JsonSerializer.Deserialize<FixerResponse>(responseContent, new JsonSerializerOptions
{
	PropertyNameCaseInsensitive = true
});

if (fixerResponse != null && fixerResponse.Success)
{
	if (fixerResponse.Rates.TryGetValue(baseCurrency, out decimal baseRate) && fixerResponse.Rates.TryGetValue(targetCurrency, out decimal targetRate))
	{
		// Convert baseCurrency to EUR
		decimal amountInEur = amount / baseRate;
		// Convert EUR to targetCurrency
		decimal convertedAmount = amountInEur * targetRate;
		Console.WriteLine($"{amount} {baseCurrency} is equal to {convertedAmount} {targetCurrency} on {fixerResponse.Date}");
	}
	else
	{
		Console.WriteLine($"Conversion rates for {baseCurrency} or {targetCurrency} not found.");
	}
}
else
{
	Console.WriteLine("Failed to retrieve exchange rate data.");
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
