using SXCore.Common.Interfaces;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace SXCore.Common.Contracts
{
    /// <summary>
    /// Represents Querable Repository class,
    /// for that IRepositoryQuery is appliable and can be computed
    /// </summary>
    /// <typeparam name="T">Type of the Enity, retured by the Repository</typeparam>
    public interface IQuerableRepository<T> where T : class
    {
        /// <summary>
        /// Makes the Query that holds conditions to be executed
        /// </summary>
        /// <returns>Query that (being executed) can return Entity objects from Repository</returns>
        IRepositoryQuery<T> MakeQuery();

        /// <summary>
        /// Executes the Query (compute Entities by the Query) on the current Repository
        /// </summary>
        /// <param name="q">Query to be executed</param>
        /// <returns>Collection of Entities, that were computed by Query</returns>
        IEnumerable<T> Execute(IRepositoryQuery<T> q);
        /// <summary>
        /// Executes the Query (compute Entities by the Query) on the current Repository
        /// </summary>
        /// <param name="q">Query to be executed</param>
        /// <returns>Collection of Entities, that were computed by Query</returns>
        Task<IEnumerable<T>> ExecuteAsync(IRepositoryQuery<T> q);
    }
}
