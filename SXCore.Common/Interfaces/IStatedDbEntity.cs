using SXCore.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Interfaces
{
    public interface IStatedDbEntity
    {
        DbState DbState { get; }

        void ChangeDbState(DbState state);
        void DeleteFromDb();
    }
}
