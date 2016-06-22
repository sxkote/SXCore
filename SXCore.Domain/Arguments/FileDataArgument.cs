using SXCore.Common.Values;
using System;

namespace SXCore.Domain.Arguments
{
    public class FileDataArgument: EventArgs
    {
        public FileData FileData { get; set; }

        public FileDataArgument(FileData fileData)
        {
            this.FileData = fileData;
        }
    }
}
