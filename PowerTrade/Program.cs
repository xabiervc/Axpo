using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerTrade.Models;
using PowerTrade.Services;
using PowerTrade.Services.Implementations;

var host = Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, config) =>
               {
                   config.SetBasePath(AppContext.BaseDirectory);
                   config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                   config.AddCommandLine(args);
               })
               .ConfigureServices((context, services) =>
               {
                   services.Configure<ConfigModel>(context.Configuration);
                   services.AddSingleton<IConfiguration>(context.Configuration);
                   services.AddSingleton<Axpo.PowerService>();
                   services.AddSingleton<ITradeService, TradeService>();
                   services.AddSingleton<IAggregatorService, AggregatorService>();
                   services.AddSingleton<ICsvGeneratorService, CsvGeneratorService>();
                   services.AddSingleton<IScheduler, Scheduler>();
                   services.AddLogging(configure => configure.AddConsole());
               }).Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var configuration = host.Services.GetRequiredService<IConfiguration>();
var configModel = configuration.Get<ConfigModel>();

Console.WriteLine("The default time zone is 'Europe/Berlin'. Do you want to change it? (yes/no)");
var response = Console.ReadLine()?.Trim().ToLower();

string timeZoneId = configModel.DefaultTimeZone;

if (response == "yes")
{
    Console.WriteLine("Please enter the new time zone (e.g. 'Europe/Istanbul'):");
    var inputTimeZone = Console.ReadLine()?.Trim();

    if (!string.IsNullOrEmpty(inputTimeZone))
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(inputTimeZone);
            timeZoneId = inputTimeZone;
        }
        catch (Exception ex) when(ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException)
        {
            logger.LogWarning("The provided time zone '{TimeZone}' is not valid. Using the default time zone 'Europe/Berlin'.", inputTimeZone);
            logger.LogWarning(ex.Message);
        }
    }
}

var scheduler = host.Services.GetRequiredService<IScheduler>();
await scheduler.StartAsync(timeZoneId);

await host.RunAsync();