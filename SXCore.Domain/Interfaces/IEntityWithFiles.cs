using SXCore.Domain.Entities;
using System.Collections.Generic;

namespace SXCore.Domain.Interfaces
{


    public interface IEntityWithFiles
    {
        ICollection<FileBlob> Files { get; }
    }
}
