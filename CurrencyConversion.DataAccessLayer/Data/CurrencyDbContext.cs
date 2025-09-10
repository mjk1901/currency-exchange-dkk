using CurrencyConversion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConversion.DataAccessLayer.Data
{
    public class CurrencyDbContext : DbContext
    {
        public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options) { }

        public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
        public DbSet<ConversionRecord> Conversions => Set<ConversionRecord>();
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
                b.HasIndex(x => x.CurrencyCode).IsUnique();
                b.Property(x => x.DkkPerUnit).HasPrecision(18,6);
                b.Property(x => x.UpdatedUtc);
                b.Property(x => x.AsOfDate);
            });

            modelBuilder.Entity<ConversionRecord>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.FromCurrency).IsRequired().HasMaxLength(10);
                b.Property(x => x.InputAmount).HasPrecision(18, 6);
                b.Property(x => x.OutputDkk).HasPrecision(18, 6);
                b.Property(x => x.RateUsed).HasPrecision(18, 6);
                b.HasIndex(x => x.FromCurrency);
                b.HasIndex(x => x.CreatedUtc);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
