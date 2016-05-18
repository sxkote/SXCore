using SXCore.Common.Entities;
using SXCore.Common.Interfaces;
using System;

namespace SXCore.Common.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        //void Rollback();
    }

    public interface ICoreUnitOfWork : IUnitOfWork
    {
        T FindByID<T>(long id) where T : class;
        T FindByCode<T>(string code) where T : class, ICoded;
        T Find<T>(object key) where T : class, ICoded;
        T FindType<T>(object key) where T : Types;
        T Create<T>(T entity) where T : class;
    }
}
