namespace SXCore.Common.Interfaces
{
    /// <summary>
    /// Represents Identifiable class with the ID field of generic type
    /// </summary>
    /// <typeparam name="T">Type of the ID field</typeparam>
    public interface IIdentifiable<T>
    {
        T ID { get; }
    }

}
