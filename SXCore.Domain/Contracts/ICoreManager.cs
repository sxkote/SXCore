using SXCore.Common.Contracts;

namespace SXCore.Domain.Contracts
{
    public interface ICoreManager : IManager
    {
        void SaveChanges();
    }
}
