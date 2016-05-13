using SXCore.Common.Classes;
using SXCore.Common.Contracts;
using SXCore.Common.Entities;
using SXCore.Common.Interfaces;
using SXCore.Infrastructure.EF.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SXCore.Infrastructure.EF.Services
{
    public class CoreRepository<TContext, TEntity> : ICoreRepository<TEntity>
    where TContext : CoreDbContext
    where TEntity : class, IEntity
    {
        protected bool _autoSave = false;

        #region Properties
        public bool AutoSave { get { return _autoSave; } }

        public TContext DbContext { get; private set; }
        public DbSet<TEntity> DbSet { get; private set; }

        protected virtual IQueryable<TEntity> Query
        { get { return this.DbSet; } }

        protected virtual IQueryable<TEntity> QueryAll
        { get { return this.Query; } }

        protected virtual IQueryable<TEntity> QuerySingle
        { get { return this.Query; } }
        #endregion

        #region Constructor
        public CoreRepository(TContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<TEntity>();
        }
        #endregion

        #region IQuerableRepository<T>
        public IRepositoryQuery<TEntity> MakeQuery()
        {
            return new RepositoryQuery<TEntity>(this);
        }

        protected virtual IQueryable<TEntity> ExecuteQuery(IRepositoryQuery<TEntity> q)
        {
            var query = this.Query;

            if (q.Includes != null)
                foreach (var i in q.Includes)
                    query = query.Include(i);

            //apply selected Filter
            if (q.Condition != null)
                query = query.Where(q.Condition);

            //apply Order
            if (q.Order != null)
                query = q.Order(query);

            //apply Skip
            if (q.SkipCount != null && q.SkipCount.HasValue)
                query = query.Skip(q.SkipCount.Value);

            //apply Take
            if (q.TakeCount != null && q.TakeCount.HasValue)
                query = query.Take(q.TakeCount.Value);

            return query;
        }

        public IEnumerable<TEntity> Execute(IRepositoryQuery<TEntity> q)
        {
            return this.ExecuteQuery(q).ToList();
        }

        public async Task<IEnumerable<TEntity>> ExecuteAsync(IRepositoryQuery<TEntity> q)
        {
            return await this.ExecuteQuery(q).ToListAsync();
        }
        #endregion

        #region Functions
        protected virtual TEntity Get(object key, params Expression<Func<TEntity, object>>[] includes)
        {
            if (key == null)
                return null;

            var query = this.QuerySingle;

            if (includes != null)
                foreach (var incl in includes)
                    if (incl != null)
                        query = query.Include(incl);

            TEntity result = null;

            if (key is long && (long)key > 0)
            {
                return query.SingleOrDefault(o => o.ID == (long)key);
            }

            //if key is a string value
            var code = key.ToString();
            if (String.IsNullOrEmpty(code))
                return null;

            if (typeof(ICoded).IsAssignableFrom(typeof(TEntity)))
            {
                result = query.SingleOrDefault(BuildFindExpression("Code", code));
                if (result != null)
                    return result;
            }

            if (typeof(IType).IsAssignableFrom(typeof(TEntity)))
            {
                result = query.SingleOrDefault(BuildFindExpression("Name", code));
                if (result != null)
                    return result;
            }

            return null;
        }

        protected void RemoveAvatar(IEntityWithAvatar entity)
        {
            if (entity == null)
                return;

            var entry = this.DbContext.Entry(entity);

            entry.Reference(s => s.Avatar).Load();

            if (entry.Entity.Avatar != null)
            {
                this.DbContext.Set<Avatar>().Remove(entry.Entity.Avatar);
                entry.Entity.ChangeAvatar(null);
            }
        }

        protected void RemoveFileBlob(IEntityWithFile entity)
        {
            if (entity == null)
                return;

            var entry = this.DbContext.Entry(entity);

            entry.Reference(s => s.File).Load();

            if (entry.Entity.File != null)
            {
                this.DbContext.Set<FileBlob>().Remove(entry.Entity.File);
                entry.Entity.ChangeFile(null);
            }
        }

        protected void RemoveFileBlobs(IEntityWithFiles entity)
        {
            if (entity == null)
                return;

            var entry = this.DbContext.Entry(entity);

            entry.Collection(s => s.Files).Load();

            if (entry.Entity.Files != null)
            {
                this.DbContext.Set<FileBlob>().RemoveRange(entry.Entity.Files);
                entry.Entity.Files.Clear();
            }
        }

        protected void LoadReference<TItem>(TEntity entity, Expression<Func<TEntity, TItem>> navigation, Expression<Func<TItem, bool>> filter, params Expression<Func<TItem, object>>[] includes)
            where TItem : class
        {
            var entry = this.DbContext.Entry(entity);
            if (entity != null && navigation != null)
            {
                var query = entry.Reference(navigation).Query();

                if (filter != null)
                    query = query.Where(filter);

                if (includes != null)
                    foreach (var include in includes)
                        if (include != null)
                            query = query.Include(include);

                query.Load();
            }
        }

        protected void LoadCollection<TItem>(TEntity entity, Expression<Func<TEntity, ICollection<TItem>>> collection, Expression<Func<TItem, bool>> filter, params Expression<Func<TItem, object>>[] includes)
            where TItem : class
        {
            var entry = this.DbContext.Entry(entity);
            if (entity != null && collection != null)
            {
                var query = entry.Collection(collection).Query();

                if (filter != null)
                    query = query.Where(filter);

                if (includes != null)
                    foreach (var include in includes)
                        if (include != null)
                            query = query.Include(include);

                query.Load();
            }
        }
        #endregion

        #region IRepository<T>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return this.QueryAll.ToList();
        }

        public virtual TEntity Get(object id)
        {
            return this.Get(id, null);
        }

        public virtual TEntity Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity", "Can't add null Entity to DbContext");

            DbEntityEntry dbEntityEntry = this.DbContext.Entry(entity);

            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                this.DbSet.Add(entity);
            }

            if (this.AutoSave)
                this.DbContext.SaveChanges();

            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity", "Can't update null Entity to DbContext");

            DbEntityEntry dbEntityEntry = this.DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            dbEntityEntry.State = EntityState.Modified;

            if (this.AutoSave)
                this.DbContext.SaveChanges();

            return entity;
        }

        public virtual TEntity Modify(TEntity entity)
        {
            if (entity.ID > 0)
                return this.Update(entity);

            return this.Add(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity", "Can't delete null Entity to DbContext");

            if (entity is IStatedObject)
            {
                (entity as IStatedObject).MarkDeleted();
            }
            else
            {
                if (entity is IEntityWithAvatar)
                    this.RemoveAvatar(entity as IEntityWithAvatar);
                if (entity is IEntityWithFile)
                    this.RemoveFileBlob(entity as IEntityWithFile);

                var dbEntityEntry = this.DbContext.Entry(entity);

                if (dbEntityEntry.State != EntityState.Deleted)
                {
                    dbEntityEntry.State = EntityState.Deleted;
                }
                else
                {
                    this.DbSet.Attach(entity);
                    this.DbSet.Remove(entity);
                }
            }

            if (this.AutoSave)
                this.DbContext.SaveChanges();
        }

        public virtual void Delete(object id)
        {
            var entity = this.Get(id);
            if (entity == null)
                return;

            this.Delete(entity);
        }
        #endregion

        #region Statics
        /// <summary>
        /// Builds Lambda Expression for IQuerable&lt;T&gt; (DbSet) to Find the entity by Field and Value
        /// </summary>
        /// <param name="field">Name of the field to search</param>
        /// <param name="value">Value of the field to search</param>
        /// <returns>Lambda Expression to make a query</returns>
        static protected Expression<Func<TEntity, bool>> BuildFindExpression(string field, object value)
        {
            var entityExpression = Expression.Parameter(typeof(TEntity));

            var fieldExpression = Expression.Equal(Expression.Property(entityExpression, field), Expression.Constant(value));

            //var subscriptionExpression = Expression.Equal(Expression.Property(entityExpression, "SubscriptionID"), Expression.Constant(this.Subscriber.SubscriptionID));
            //var fullExpression = Expression.And(fieldExpression, subscriptionExpression);

            return Expression.Lambda<Func<TEntity, bool>>(fieldExpression, entityExpression);
        }
        #endregion
    }
}
