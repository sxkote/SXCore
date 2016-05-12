using SXCore.Common.Contracts;
using SXCore.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SXCore.Common.Classes
{
    public sealed class RepositoryQuery<T> : IRepositoryQuery<T>
    where T : class
    {
        #region Variables
        private readonly IQuerableRepository<T> _repository;

        private readonly List<Expression<Func<T, object>>> _includes;

        private Expression<Func<T, bool>> _condition;
        private Func<IQueryable<T>, IOrderedQueryable<T>> _order_by;

        //private Func<T, object> _projection;

        private int? _skipCount;
        private int? _takeCount;
        #endregion

        #region Properties
        public IQuerableRepository<T> Repository
        { get { return _repository; } }

        public List<Expression<Func<T, object>>> Includes
        { get { return _includes; } }

        public Expression<Func<T, bool>> Condition
        { get { return _condition; } }

        public Func<IQueryable<T>, IOrderedQueryable<T>> Order
        { get { return _order_by; } }

        //public Func<T, object> Projection 
        //{ get { return _projection; } }

        public int? SkipCount
        { get { return _skipCount; } }

        public int? TakeCount
        { get { return _takeCount; } }
        #endregion

        #region Constructor
        public RepositoryQuery(IQuerableRepository<T> repository)
        {
            _repository = repository;
            _includes = new List<Expression<Func<T, object>>>();
        }
        #endregion

        #region Methods
        public IRepositoryQuery<T> Where(Expression<Func<T, bool>> condition)
        {
            _condition = condition;
            return this;
        }

        public IRepositoryQuery<T> OrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            _order_by = orderBy;
            return this;
        }

        public IRepositoryQuery<T> Include(Expression<Func<T, object>> expression)
        {
            _includes.Add(expression);
            return this;
        }

        //public IRepositoryQuery<T> Project(Func<T, object> projection)
        //{
        //    _projection = projection;
        //    return this;
        //}

        public IRepositoryQuery<T> Skip(int skip)
        {
            _skipCount = skip;
            return this;
        }

        public IRepositoryQuery<T> Take(int take)
        {
            _takeCount = take;
            return this;
        }

        public IRepositoryQuery<T> Page(int page_number, int page_size)
        {
            _skipCount = (page_number - 1) * page_size;
            _takeCount = page_size;
            return this;
        }
        #endregion

        #region Execute
        public IEnumerable<T> Execute()
        {
            return _repository.Execute(this);
        }

        public async Task<IEnumerable<T>> ExecuteAsync()
        {
            return await _repository.ExecuteAsync(this);
        }
        #endregion
    }
}
