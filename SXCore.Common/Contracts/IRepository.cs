using SXCore.Common.Interfaces;
using System.Collections.Generic;

namespace SXCore.Common.Contracts
{
    public interface IRepository { }

    public interface IRepository<T> : IRepository
         where T : class
    {
        bool AutoSave { get; }

        IEnumerable<T> GetAll();

        T Get(object id);

        T Add(T entity);

        T Update(T entity);

        T Modify(T entity);

        void Delete(T entity);

        void Delete(object id);
    }
}
