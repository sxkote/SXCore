using SXCore.Common.Contracts;
using SXCore.Common.Interfaces;
using SXCore.Domain.Entities;

namespace SXCore.Domain.Contracts
{
    public interface ICoreUnitOfWork : IUnitOfWork
    {
        T FindByID<T>(long id) where T : class;
        T FindByCode<T>(string code) where T : class, ICoded;
        T Find<T>(object key) where T : class, ICoded;
        T FindType<T>(object key) where T : Types;
        T Create<T>(T entity) where T : class;
    }
}
