using Microsoft.Extensions.Options;
using PowerTrade.Models;

namespace PowerTrade.Services
{
    public interface ICsvGeneratorService
    {
        void Generate(IEnumerable<AggregatedTradeModel> data);
    }
}