using SXCore.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Entities
{
    public abstract class SubscriptionedEntity<T, S> : IdentifiableEntity<T>, ISubscriptioned<S>
    {
        public S SubscriptionID { get; protected set; }

        S ISubscriptioned<S>.InSubscription(S subscriptionID)
        {
            if (this.SubscriptionID.Equals(default(S)))
                this.SubscriptionID = subscriptionID;
            return this.SubscriptionID;
        }
    }
}
