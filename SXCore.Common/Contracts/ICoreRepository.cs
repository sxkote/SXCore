using SXCore.Common.Interfaces;

namespace SXCore.Common.Contracts
{
    public interface ICoreRepository<T> : IRepository<T>, IQuerableRepository<T>
        where T : class, IEntity
    { }
}
