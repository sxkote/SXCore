using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Interfaces
{
    public interface ISubscriber<S, P, M>
    {
        S SubscriptionID { get; }
        P PersonID { get; }
        M ManagerID { get; }
    }
}
