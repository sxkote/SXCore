using SXCore.Common.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Domain.Values
{
    public class UploadFileDataInfo
    {
        public FileData FileData { get; set; }

        public UploadFileDataInfo(FileData fileData)
        {
            this.FileData = fileData;
        }
    }
}
