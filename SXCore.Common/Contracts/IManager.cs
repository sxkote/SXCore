using System;

namespace SXCore.Common.Contracts
{
    public interface IManager : IDisposable
    {
    }

    public interface ICoreManager: IManager
    {
        void SaveChanges();
    }
}
