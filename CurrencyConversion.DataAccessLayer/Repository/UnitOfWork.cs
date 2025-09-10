using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.DataAccessLayer.Data;
using CurrencyConversion.DataAccessLayer.Repository;
using CurrencyConversion.DataAccessLayer.Repository.IRepository;
using CurrencyConversion.Service.Repository.IRepository;

namespace CurrencyConversion.Service.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CurrencyDbContext _currencyDbContext;
        public IExchangeRatesRepository ExchangeRates { get; private set; }
        public IConversionRepository Conversions { get; private set; }
        public IApplicationUserRepository Users { get; private set; }

        public UnitOfWork(CurrencyDbContext currencyDbContext)
        {
            _currencyDbContext = currencyDbContext;
            ExchangeRates = new ExchangeRatesRepository(_currencyDbContext);
            Conversions = new ConversionRepository(_currencyDbContext);
            Users = new ApplicationUserRepository(_currencyDbContext);
        }

        public void Save()
        {            
            _currencyDbContext.SaveChanges();
        }
    }
}
