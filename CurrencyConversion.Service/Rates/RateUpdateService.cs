using CurrencyConversion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using log4net;
using CurrencyConversion.DataAccessLayer.Data;

namespace CurrencyConversion.Service.Rates
{
    public class RateUpdateService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly log4net.ILog _log;
        private readonly TimeSpan _interval;

        public RateUpdateService(IServiceProvider sp, IConfiguration cfg)
        {
            _sp = sp;
            _log = LogManager.GetLogger(typeof(RateUpdateService));
            var minutes = Convert.ToInt32(cfg["Nationalbanken:RefreshMinutes"]);
            _interval = TimeSpan.FromMinutes(minutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await UpdateOnce(stoppingToken);
            using var timer = new PeriodicTimer(_interval);
            while (await timer.WaitForNextTickAsync(stoppingToken))
                await UpdateOnce(stoppingToken);
        }

        private async Task UpdateOnce(CancellationToken ct)
        {
            using var scope = _sp.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<INationalbankenClient>();
            var db = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();
            var (asOf, map) = await client.GetLatestAsync(ct);

            foreach (var (code, dkkPerUnit) in map)
            {
                var entity = await db.ExchangeRates.SingleOrDefaultAsync(x => x.CurrencyCode == code, ct);
                if (entity == null)
                    db.ExchangeRates.Add(new ExchangeRate { CurrencyCode = code, DkkPerUnit = dkkPerUnit, AsOfDate = asOf, UpdatedUtc = DateTime.UtcNow });
                else
                {
                    entity.DkkPerUnit = dkkPerUnit;
                    entity.AsOfDate = asOf;
                    entity.UpdatedUtc = DateTime.UtcNow;
                }
            }
            await db.SaveChangesAsync(ct);
            _log.Info($"Rates updated: {map.Count}");
        }
    }
}
