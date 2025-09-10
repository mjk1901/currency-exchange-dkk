using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.DataAccessLayer.Data;
using CurrencyConversion.Service.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConversion.Service.Repository
{
    public class Repository<T> : IRepository<T>  where T : class
    {
        private readonly CurrencyDbContext _currencyDbContext;
        internal DbSet<T> DbSet;

        public Repository(CurrencyDbContext currencyDbContext)
        {
            _currencyDbContext = currencyDbContext;
            this.DbSet = _currencyDbContext.Set<T>();
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public Task<List<T>> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);

            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.
                    Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.AsNoTracking().ToListAsync();
        }

        public Task<T> Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = DbSet;
            }
            else
            {
                query = DbSet.AsNoTracking();
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.
                    Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            query = query.Where(filter);
            return query.FirstOrDefaultAsync();

        }
    }
}
