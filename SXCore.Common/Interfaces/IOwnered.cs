namespace SXCore.Common.Interfaces
{
    public interface IOwnered
    {
        long OwnerID { get; }
        int OwnerTypeID { get; }
    }
}
