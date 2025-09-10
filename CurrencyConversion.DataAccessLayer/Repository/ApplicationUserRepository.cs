using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.DataAccessLayer.Data;
using CurrencyConversion.DataAccessLayer.Repository.IRepository;
using CurrencyConversion.Domain.Entities;
using CurrencyConversion.Service.Repository;
using CurrencyConversion.Service.Repository.IRepository;

namespace CurrencyConversion.DataAccessLayer.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly CurrencyDbContext _currencyDbContext;

        public ApplicationUserRepository(CurrencyDbContext currencyDbContext) : base(currencyDbContext)
        {
            _currencyDbContext = currencyDbContext;
        }
    }
}
