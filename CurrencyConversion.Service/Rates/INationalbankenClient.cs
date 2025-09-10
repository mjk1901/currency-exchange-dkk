using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConversion.Service.Rates
{
    public interface INationalbankenClient
    {
        Task<(DateTime asOfDate, Dictionary<string, decimal> dkkPerUnit)> GetLatestAsync(CancellationToken ct);
    }
}
