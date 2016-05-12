namespace SXCore.Common.Contracts
{
    public interface IDependencyRegistrator
    {
        void RegisterType<TService, TInterface>(Enums.DependencyScope scope = Enums.DependencyScope.Default) where TService : TInterface;
        void RegisterInstance<TInterface>(TInterface service) where TInterface : class;
    }
}
