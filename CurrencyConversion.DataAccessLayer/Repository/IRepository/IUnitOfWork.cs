using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.DataAccessLayer.Repository.IRepository;

namespace CurrencyConversion.Service.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IExchangeRatesRepository ExchangeRates { get; }
        IConversionRepository Conversions { get; }
        IApplicationUserRepository Users { get; }
        void Save();
    }
}
