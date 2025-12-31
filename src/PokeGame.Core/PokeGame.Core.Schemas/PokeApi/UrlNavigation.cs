namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Allows for automatic fetching of a resource via a url
    /// </summary>
    public abstract record UrlNavigation<T> where T : ResourceBase
    {
        /// <summary>
        /// Url of the referenced resource
        /// </summary>
        public required string Url { get; set; }
    }
}
