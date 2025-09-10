using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.Domain.Entities;

namespace CurrencyConversion.Service.Repository.IRepository
{
    public interface IExchangeRatesRepository : IRepository<ExchangeRate>
    {
    }
}
