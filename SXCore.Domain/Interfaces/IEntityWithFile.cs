using SXCore.Domain.Entities;

namespace SXCore.Domain.Interfaces
{
    public interface IEntityWithFile
    {
        FileBlob File { get; }
        void ChangeFile(FileBlob file);
    }
}
