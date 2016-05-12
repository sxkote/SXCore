using SXCore.Common.Interfaces;
using System;

namespace SXCore.Common.Values
{
    public class Subscriber<S, P, M> : ISubscriber<S, P, M>
    {
        private S _subscriptionID;
        private P _personID;
        private M _managerID;

        public S SubscriptionID
        {
            get { return _subscriptionID; }
        }

        public P PersonID
        {
            get { return _personID; }
        }

        public M ManagerID
        {
            get { return _managerID; }
        }

        public Subscriber(S subscriptionID, P personID, M managerID)
        {
            _subscriptionID = subscriptionID;
            _personID = personID;
            _managerID = managerID;
        }

        public Subscriber(ISubscriber<S, P, M> provider)
        {
            if (provider == null)
                throw new ArgumentNullException();

            _subscriptionID = provider.SubscriptionID;
            _managerID = provider.ManagerID;
            _personID = provider.PersonID;
        }
    }

    public class Subscriber : Subscriber<long, long, long>, ISubscriber<long, long, long>
    {
        public Subscriber(long subscriptionID, long personID, long managerID)
            : base(subscriptionID, personID, managerID)
        { }

        public Subscriber(ISubscriber provider)
            : base(provider)
        { }
    }
}
