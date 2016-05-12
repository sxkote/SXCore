namespace SXCore.Common.Interfaces
{
    public interface ISubscriber<S, P, M>
    {
        S SubscriptionID { get; }
        P PersonID { get; }
        M ManagerID { get; }
    }

    public interface ISubscriber : ISubscriber<long, long, long> { }
}
