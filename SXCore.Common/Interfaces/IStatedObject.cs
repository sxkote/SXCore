using SXCore.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Interfaces
{
    public interface IStatedObject
    {
        ObjectState State { get; }

        void ChangeState(ObjectState state);
        void MarkDeleted();
        bool IsDeleted();
    }
}
