using SXCore.Common.Entities;
using System.Collections.Generic;

namespace SXCore.Common.Interfaces
{
    public interface IEntity : IIdentifiable { }

    public interface IEntityWithAvatar
    {
        Avatar Avatar { get; }
        void ChangeAvatar(Avatar avatar);
    }

    public interface IEntityWithFile
    {
        FileBlob File { get; }
        void ChangeFile(FileBlob file);
    }

    public interface IEntityWithFiles
    {
        ICollection<FileBlob> Files { get; }
    }
}
