using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.Domain.Entities;
using CurrencyConversion.DataAccessLayer.Data;
using CurrencyConversion.Service.Repository.IRepository;

namespace CurrencyConversion.Service.Repository
{
    public class ConversionRepository : Repository<ConversionRecord>, IConversionRepository
    {
        private readonly CurrencyDbContext _currencyDbContext;

        public ConversionRepository(CurrencyDbContext currencyDbContext) : base(currencyDbContext)
        {
            _currencyDbContext = currencyDbContext;
        }
    }
}
