using Microsoft.Extensions.Options;
using PowerTrade.Extensions;
using PowerTrade.Models;
using System.Globalization;
using System.Text;

namespace PowerTrade.Services.Implementations
{
    public class CsvGeneratorService(IOptions<ConfigModel> config) : ICsvGeneratorService
    {
        private ConfigModel Config => config.Value;

        public void Generate(IEnumerable<AggregatedTradeModel> data)
        {
            var filePath = GetFullFilePathName();
            var directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("Datetime,Volume");
                foreach (var entry in data)
                {
                    writer.WriteLine($"{entry.DateTime.ToLocalDateTime(Config.TimeZoneInfo):yyyy-MM-ddTHH:mm:ssZ},{entry.Volume.ToString(CultureInfo.InvariantCulture)}");
                }
            }
        }

        public string GetFullFilePathName()
        {
            var utcDateTimeNow = DateTimeOffset.UtcNow.UtcDateTime;
            return string.Format(Config.File.FilePathName, utcDateTimeNow.ToLocalDateTime(Config.TimeZoneInfo).AddDays(1).ToString("yyyyMMdd"), utcDateTimeNow.ToLocalDateTime(Config.TimeZoneInfo).ToString("yyyyMMddHHmm"));
        }
    }
}