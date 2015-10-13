using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Interfaces
{
    public interface ISubscriptioned<T>
    {
        T SubscriptionID { get; }
        T InSubscription(T subscriptionID);
    }
}
