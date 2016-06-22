using SXCore.Domain.Enums;

namespace SXCore.Domain.Interfaces
{
    public interface IStatedEntity
    {
        EntityState State { get; }

        void ChangeState(EntityState state);
        void MarkDeleted();
        bool IsActiveState();
    }
}
