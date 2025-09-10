using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.Domain.Entities;
using CurrencyConversion.Service.Repository.IRepository;

namespace CurrencyConversion.DataAccessLayer.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
    }
}
