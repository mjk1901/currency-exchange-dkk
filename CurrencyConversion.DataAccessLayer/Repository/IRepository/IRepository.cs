using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConversion.Service.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperty = null);
        Task<T> Get(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracked = false);
        void Add(T entity);
    }
}
