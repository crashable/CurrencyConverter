using System.Text.Json;

string baseCurrency = args[0];
string targetCurrency = args[1];

HttpClient httpClient = new HttpClient();
string apiKey = Environment.GetEnvironmentVariable("FIXERIO_API_KEY") ?? throw new InvalidOperationException("API key not found in environment variables.");
string _baseUrl = "https://data.fixer.io/api/";
//https://data.fixer.io/api/latest?base=EUR&symbols=NOK

httpClient.BaseAddress = new Uri(_baseUrl);

if (!decimal.TryParse(args[2], out decimal amount))
{
    Console.WriteLine("Invalid amount. Please provide a valid number for the amount.");
    return;
}
HttpResponseMessage response = await httpClient.GetAsync($"latest?access_key={apiKey}&base={baseCurrency}&symbols={targetCurrency}");
response.EnsureSuccessStatusCode();

string responseContent = await response.Content.ReadAsStringAsync();

FixerResponse? fixerResponse = JsonSerializer.Deserialize<FixerResponse>(responseContent, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

if (fixerResponse != null && fixerResponse.Success)
{
    if (fixerResponse.Rates.TryGetValue(targetCurrency, out decimal rate))
    {
        decimal convertedAmount = amount * rate;
        Console.WriteLine($"{amount} {baseCurrency} is equal to {convertedAmount} {targetCurrency}");
    }
    else
    {
        Console.WriteLine($"Conversion rate for {targetCurrency} not found.");
    }
}
else
{
    Console.WriteLine("Failed to retrieve exchange rate data.");
}





public class FixerResponse
{
    public bool Success { get; set; }
    public long Timestamp { get; set; }
    public string Base { get; set; }
    public string Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}








