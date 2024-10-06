using PowerTrade.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerTrade.Extensions;

namespace PowerTrade.Services.Implementations
{
    public class Scheduler(ILogger<Scheduler> logger, ITradeService tradeService, IAggregatorService aggregatorService, ICsvGeneratorService csvGenerator, IOptions<ConfigModel> config) : IScheduler
    {
        private ConfigModel Config => config.Value;

        private Timer _timer;

        public async Task StartAsync(string timeZoneId)
        {
            Config.TimeZone = timeZoneId;
            logger.LogInformation("Scheduler started.");
            await GenerateReport();

            ScheduleNextReport(DateTime.UtcNow.ToLocalDateTime(Config.TimeZoneInfo));
        }

        private async Task GenerateReport()
        {
            int maximumRetries = Config.Retry.Maximum;
            int delay = Config.Retry.DelayMiliseconds;
            int attempt = 0;
            bool success = false;

            while (attempt < maximumRetries && !success)
            {
                try
                {
                    var currentDateTime = DateTime.UtcNow.ToLocalDateTime(Config.TimeZoneInfo);
                    var trades = await tradeService.GetTradesAsync(currentDateTime.AddDays(1));
                    var aggregatedData = aggregatorService.AggregateTrades(trades);
                    csvGenerator.Generate(aggregatedData);
                    logger.LogInformation("Report generated successfully.");
                    success = true;
                    Config.NextReport = currentDateTime.AddMinutes(Config.IntervalMinutes);
                    logger.LogInformation($"Next report scheduled at: {Config.NextReport}");
                    ScheduleNextReport(currentDateTime);
                }
                catch (Exception ex)
                {
                    attempt++;
                    logger.LogError(ex, $"An error occurred while generating the report. Attempt {attempt} of {maximumRetries}.");

                    if (attempt < maximumRetries)
                    {
                        await Task.Delay(delay);
                    }
                    else
                    {
                        logger.LogError(ex, "Failed to generate report after multiple attempts.");
                        throw;
                    }
                }
            }
        }

        private void ScheduleNextReport(DateTime currentDateTime)
        {
            var nextReportTime = Config.NextReport;
            var timeUntilNextReport = nextReportTime - currentDateTime;

            if (timeUntilNextReport.TotalMilliseconds <= 0)
            {
                timeUntilNextReport = TimeSpan.Zero;
            }

            _timer?.Dispose();
            _timer = new Timer(async _ => await GenerateReport(), null, timeUntilNextReport, Timeout.InfiniteTimeSpan);
        }
    }
}