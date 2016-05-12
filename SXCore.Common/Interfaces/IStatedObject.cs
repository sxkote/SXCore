using SXCore.Common.Enums;

namespace SXCore.Common.Interfaces
{
    public interface IStatedObject
    {
        ObjectState State { get; }

        void ChangeState(ObjectState state);
        void MarkDeleted();
        bool IsActiveState();
    }
}
