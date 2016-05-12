namespace SXCore.Common.Interfaces
{
    public interface ISubscriptioned<T>
    {
        T SubscriptionID { get; }
    }

    public interface ISubscriptioned : ISubscriptioned<long>
    {
    }
}
