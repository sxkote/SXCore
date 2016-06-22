using SXCore.Common.Contracts;
using SXCore.Common.Interfaces;

namespace SXCore.Domain.Contracts
{
    public interface ICoreRepository<T> : IRepository<T>, IQuerableRepository<T>
    where T : class, IEntity
    { }
}
