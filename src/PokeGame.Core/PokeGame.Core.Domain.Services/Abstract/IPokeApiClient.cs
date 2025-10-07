using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Domain.Services.Abstract
{
    /// <summary>
    /// Interface for getting data from the PokeAPI service
    /// </summary>
    internal interface IPokeApiClient
    {
        /// <summary>
        /// Gets a resource by id
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="id">Id of resource</param>
        /// <returns>The object of the resource</returns>
        Task<T> GetResourceAsync<T>(int id)
            where T : ResourceBase;

        /// <summary>
        /// Gets a resource by id
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="id">Id of resource</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The object of the resource</returns>
        Task<T> GetResourceAsync<T>(int id, CancellationToken cancellationToken)
            where T : ResourceBase;

        /// <summary>
        /// Gets a resource by name. This lookup is case insensitive.
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="name">Name of resource</param>
        /// <returns>The object of the resource</returns>
        Task<T> GetResourceAsync<T>(string name)
            where T : NamedApiResource;

        /// <summary>
        /// Gets a resource by name. This lookup is case insensitive.
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="name">Name of resource</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The object of the resource</returns>
        Task<T> GetResourceAsync<T>(string name, CancellationToken cancellationToken)
            where T : NamedApiResource;

        /// <summary>
        /// Resolves all navigation properties in a collection
        /// </summary>
        /// <typeparam name="T">Navigation type</typeparam>
        /// <param name="collection">The collection of navigation objects</param>
        /// <returns>A list of resolved objects</returns>
        Task<List<T>> GetResourceAsync<T>(IEnumerable<UrlNavigation<T>> collection)
            where T : ResourceBase;

        /// <summary>
        /// Resolves all navigation properties in a collection
        /// </summary>
        /// <typeparam name="T">Navigation type</typeparam>
        /// <param name="collection">The collection of navigation objects</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>A list of resolved objects</returns>
        Task<List<T>> GetResourceAsync<T>(
            IEnumerable<UrlNavigation<T>> collection,
            CancellationToken cancellationToken
        )
            where T : ResourceBase;

        /// <summary>
        /// Resolves a single navigation property
        /// </summary>
        /// <typeparam name="T">Navigation type</typeparam>
        /// <param name="urlResource">The single navigation object to resolve</param>
        /// <returns>A resolved object</returns>
        Task<T> GetResourceAsync<T>(UrlNavigation<T> urlResource)
            where T : ResourceBase;

        /// <summary>
        /// Resolves a single navigation property
        /// </summary>
        /// <typeparam name="T">Navigation type</typeparam>
        /// <param name="urlResource">The single navigation object to resolve</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>A resolved object</returns>
        Task<T> GetResourceAsync<T>(
            UrlNavigation<T> urlResource,
            CancellationToken cancellationToken
        )
            where T : ResourceBase;

        /// <summary>
        /// Gets a single page of named resource data
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The paged resource object</returns>
        Task<NamedApiResourceList<T>> GetNamedResourcePageAsync<T>(
            CancellationToken cancellationToken = default
        )
            where T : NamedApiResource;

        /// <summary>
        /// Gets the specified page of named resource data
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="limit">The number of resources in a list page</param>
        /// <param name="offset">Page offset</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The paged resource object</returns>
        Task<NamedApiResourceList<T>> GetNamedResourcePageAsync<T>(
            int limit,
            int offset,
            CancellationToken cancellationToken = default
        )
            where T : NamedApiResource;

        /// <summary>
        /// Gets all the named resources
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>An async enumeration of the requested resources</returns>
        IAsyncEnumerable<NamedApiResource<T>> GetAllNamedResourcesAsync<T>(
            CancellationToken cancellationToken = default
        )
            where T : NamedApiResource;

        /// <summary>
        /// Gets a single page of unnamed resource data
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The paged resource object</returns>
        Task<ApiResourceList<T>> GetApiResourcePageAsync<T>(
            CancellationToken cancellationToken = default
        )
            where T : ApiResource;

        /// <summary>
        /// Gets the specified page of unnamed resource data
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="limit">The number of resources in a list page</param>
        /// <param name="offset">Page offset</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The paged resource object</returns>
        Task<ApiResourceList<T>> GetApiResourcePageAsync<T>(
            int limit,
            int offset,
            CancellationToken cancellationToken = default
        )
            where T : ApiResource;

        /// <summary>
        /// Gets all the unnamed resources
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>An async enumeration of the requested resources</returns>
        IAsyncEnumerable<ApiResource<T>> GetAllApiResourcesAsync<T>(
            CancellationToken cancellationToken = default
        )
            where T : ApiResource;
    }
}
