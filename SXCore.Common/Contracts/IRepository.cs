using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Contracts
{
    public interface IRepository { }

    public interface IRepository<T> : IRepository
         where T : class
    {
        IEnumerable<T> GetAll();

        T Get(object id);

        T Add(T entity);

        T Update(T entity);

        T Modify(T entity);

        void Delete(T entity);

        void Delete(object id);
    }
}
