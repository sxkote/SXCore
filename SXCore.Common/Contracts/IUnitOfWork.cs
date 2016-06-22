using SXCore.Common.Interfaces;
using System;

namespace SXCore.Common.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        //void Rollback();
    }
}
