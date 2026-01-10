namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// The base record for recordes that have an API endpoint. These
    /// recordes can also be cached with their id value.
    /// </summary>
    public abstract record ResourceBase
    {
        /// <summary>
        /// The identifier for this resource
        /// </summary>
        public abstract int Id { get; init; }

        /// <summary>
        /// The endpoint string for this resource
        /// </summary>
        public static string ApiEndpoint { get; }

        /// <summary>
        /// Is endpoint case sensitive
        /// </summary>
        public static bool IsApiEndpointCaseSensitive { get; }
    }

    /// <summary>
    /// The base record for API resources that have a name property
    /// </summary>
    public abstract record NamedApiResource : ResourceBase
    {
        /// <summary>
        /// The name of this resource
        /// </summary>
        public abstract string Name { get; init; }
    }

    /// <summary>
    /// The base record for API resources that don't have a name property
    /// </summary>
    public abstract record ApiResource : ResourceBase { }
}
