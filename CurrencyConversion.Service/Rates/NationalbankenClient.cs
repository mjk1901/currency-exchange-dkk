
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace CurrencyConversion.Service.Rates
{
    public class NationalbankenClient : INationalbankenClient
    {
        private readonly HttpClient _http;
        private readonly log4net.ILog _log;
        private readonly string _url;

        public NationalbankenClient(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _log = log4net.LogManager.GetLogger(typeof(NationalbankenClient));
            _url = cfg["Nationalbanken:RatesXmlUrl"] ?? throw new ArgumentNullException("Nationalbanken:RatesXmlUrl");
        }

        public async Task<(DateTime asOfDate, Dictionary<string, decimal> dkkPerUnit)> GetLatestAsync(CancellationToken ct)
        {
            // Fetch response as string
            var response = await _http.GetStringAsync(_url, ct);

            // Parse XML
            var xml = XDocument.Parse(response);
            var root = xml.Root;

            // Read refAmount (default 1)
            decimal refAmount = 100m; // API gives rates for 100 units of currency


            // Read dailyrates date
            DateTime asOf = DateTime.UtcNow.Date;
            var dailyRates = root?.Element("dailyrates");
            if (dailyRates != null && DateTime.TryParseExact(dailyRates.Attribute("id")?.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                asOf = parsedDate;
            }

            // Build dictionary of currency rates
            var map = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            if (dailyRates != null)
            {
                foreach (var currency in dailyRates.Elements("currency"))
                {
                    var code = currency.Attribute("code")?.Value;
                    var rateStr = currency.Attribute("rate")?.Value;
                    if (!string.IsNullOrWhiteSpace(code) && decimal.TryParse(rateStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                    {
                        // Convert to per-unit rate in DKK
                        var perUnit = rate / refAmount;
                        map[code!] = decimal.Round(perUnit, 6);
                    }
                }
            }

            // Ensure DKK exists
            map["DKK"] = refAmount;

            return (asOf, map);
        }
    }    
}
