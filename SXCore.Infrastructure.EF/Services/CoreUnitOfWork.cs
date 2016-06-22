using SXCore.Common.Contracts;
using SXCore.Common.Entities;
using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Domain.Contracts;
using SXCore.Domain.Entities;
using SXCore.Infrastructure.EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Infrastructure.EF.Services
{
    public abstract class CoreUnitOfWork<TContext> : ICoreUnitOfWork
     where TContext : CoreDbContext, new()
    {
        protected TContext _dbContext;

        protected TContext DbContext
        { get { return _dbContext; } }

        public CoreUnitOfWork()
        {
            _dbContext = new TContext();
        }

        public void SaveChanges()
        {
            this.DbContext.SaveChanges();
        }

        public T FindByID<T>(long id) where T : class
        {
            return this.DbContext.Set<T>().Find(id);
        }

        public T FindByCode<T>(string code) where T : class, ICoded
        {
            return this.DbContext.Set<T>().SingleOrDefault(b => b.Code.Equals(code, CommonService.StringComparison));
        }

        public T Find<T>(object key) where T : class, ICoded
        {
            if (key == null)
                return null;

            if (key is long)
                return this.FindByID<T>((long)key);

            return this.FindByCode<T>(key.ToString());
        }

        public T FindType<T>(object key) where T : Types
        {
            if (key == null)
                return default(T);

            if (key is long)
                return this.FindByID<T>((long)key);

            var dbSet = this.DbContext.Set<T>();

            var result = dbSet.FirstOrDefault(t => t.Name.Equals(key.ToString(), StringComparison.OrdinalIgnoreCase));

            if (result == null)
                result = dbSet.FirstOrDefault(t => t.Title.Equals(key.ToString(), StringComparison.OrdinalIgnoreCase));

            return result;
        }

        public IEnumerable<T> FindTypes<T>() where T : Types
        { return this.DbContext.Set<T>().ToList(); }

        public T Create<T>(T entity) where T : class
        {
            return this.DbContext.Set<T>().Add(entity);
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }
    }
}
