using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SXCore.Common.Interfaces
{
    public interface IRepositoryQuery<T> where T : class
    {
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, bool>> Condition { get; }
        Func<IQueryable<T>, IOrderedQueryable<T>> Order { get; }
        int? SkipCount { get; }
        int? TakeCount { get; }

        IRepositoryQuery<T> Include(Expression<Func<T, object>> expression);
        IRepositoryQuery<T> Where(Expression<Func<T, bool>> condition);
        IRepositoryQuery<T> OrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        IRepositoryQuery<T> Skip(int skip);
        IRepositoryQuery<T> Take(int take);
        IRepositoryQuery<T> Page(int page_number, int page_size);

        IEnumerable<T> Execute();
        Task<IEnumerable<T>> ExecuteAsync();
    }
}
