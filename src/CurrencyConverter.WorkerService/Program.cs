using CurrencyConverter.WorkerService;
using CurrencyConverter.WorkerService.Data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<ExchangeRateContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
