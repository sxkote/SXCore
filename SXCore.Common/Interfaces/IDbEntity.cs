using SXCore.Common.Enums;

namespace SXCore.Common.Interfaces
{
    public interface IDbEntity
    {
        DbState DbState { get; }

        void ChangeDbState(DbState state);
        void DeleteFromDb();
    }
}
