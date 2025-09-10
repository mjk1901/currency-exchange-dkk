
using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            using var resp = await _http.GetAsync(_url, ct);
            resp.EnsureSuccessStatusCode();
            await using var stream = await resp.Content.ReadAsStreamAsync(ct);

            var map = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            DateTime asOfDate = DateTime.UtcNow.Date;

            var settings = new XmlReaderSettings { IgnoreWhitespace = true, Async = true };
            using var reader = XmlReader.Create(stream, settings);

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("dailyrates", StringComparison.OrdinalIgnoreCase))
                {
                    var id = reader.GetAttribute("id");
                    if (DateTime.TryParse(id, out var parsed)) asOfDate = parsed;
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("currency", StringComparison.OrdinalIgnoreCase))
                {
                    var code = reader.GetAttribute("code");
                    var rateStr = reader.GetAttribute("rate");
                    if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(rateStr))
                    {
                        if (decimal.TryParse(rateStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var ratePer100))
                        {
                            var perUnit = ratePer100 / 100m;
                            map[code!] = decimal.Round(perUnit, 6);
                        }
                    }
                }
            }

            // Ensure DKK exists
            map["DKK"] = 1.0m;
            _log.Info($"Fetched {map.Count} rates for {asOfDate.ToString("yyyy-MM-dd")}");
            return (asOfDate, map);
        }
    }    
}
