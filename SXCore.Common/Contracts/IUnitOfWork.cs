namespace SXCore.Common.Contracts
{
    public interface IUnitOfWork
    {
        void SaveChanges();
        //void Rollback();
    }
}
