using SXCore.Common.Entities;
using SXCore.Common.Enums;
using SXCore.Common.Interfaces;

namespace SXCore.Domain.Entities
{
    public abstract class Entity : IdentifiableEntity<long>, IEntity, IDbEntity
    {
        protected DbState _dbState = DbState.None;

        DbState IDbEntity.DbState { get { return _dbState; } }

        void IDbEntity.ChangeDbState(DbState state)
        { _dbState = state; }

        void IDbEntity.DeleteFromDb()
        { _dbState = DbState.Deleted; }
    }
}
