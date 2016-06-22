using SXCore.Domain.Entities;

namespace SXCore.Domain.Interfaces
{
    public interface IEntityWithAvatar
    {
        Avatar Avatar { get; }
        void ChangeAvatar(Avatar avatar);
    }
}
