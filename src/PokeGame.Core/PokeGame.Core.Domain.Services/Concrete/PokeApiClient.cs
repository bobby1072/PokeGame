using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PokeApiNet
{
    public sealed class PokeApiClient : IPokeApiClient
    {
        private readonly HttpClient _client;

        public PokeApiClient(HttpClient httpClient, string baseUrl)
        {
            _client = httpClient;
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<T> GetResourceAsync<T>(int id)
            where T : ResourceBase
        {
            return await GetResourceAsync<T>(id, CancellationToken.None);
        }

        public async Task<T> GetResourceAsync<T>(int id, CancellationToken cancellationToken)
            where T : ResourceBase
        {
            return await GetResourcesWithParamsAsync<T>(id.ToString(), cancellationToken);
        }

        public async Task<T> GetResourceAsync<T>(string name)
            where T : NamedApiResource
        {
            return await GetResourceAsync<T>(name, CancellationToken.None);
        }

        public async Task<T> GetResourceAsync<T>(string name, CancellationToken cancellationToken)
            where T : NamedApiResource
        {
            string sanitizedName = name.Replace(" ", "-") // no resource can have a space in the name; API uses -'s in their place
                .Replace("'", "") // looking at you, Farfetch'd
                .Replace(".", ""); // looking at you, Mime Jr. and Mr. Mime

            // Nidoran is interesting as the API wants 'nidoran-f' or 'nidoran-m'

            return await GetResourcesWithParamsAsync<T>(sanitizedName, cancellationToken);
        }

        public async Task<List<T>> GetResourceAsync<T>(IEnumerable<UrlNavigation<T>> collection)
            where T : ResourceBase
        {
            return await GetResourceAsync<T>(collection, CancellationToken.None);
        }

        public async Task<List<T>> GetResourceAsync<T>(
            IEnumerable<UrlNavigation<T>> collection,
            CancellationToken cancellationToken
        )
            where T : ResourceBase
        {
            return (
                await Task.WhenAll(collection.Select(m => GetResourceAsync(m, cancellationToken)))
            ).ToList();
        }

        public async Task<T> GetResourceAsync<T>(UrlNavigation<T> urlResource)
            where T : ResourceBase
        {
            return await GetResourceByUrlAsync<T>(urlResource.Url, CancellationToken.None);
        }

        public async Task<T> GetResourceAsync<T>(
            UrlNavigation<T> urlResource,
            CancellationToken cancellationToken
        )
            where T : ResourceBase
        {
            return await GetResourceByUrlAsync<T>(urlResource.Url, cancellationToken);
        }

        public Task<NamedApiResourceList<T>> GetNamedResourcePageAsync<T>(
            CancellationToken cancellationToken = default
        )
            where T : NamedApiResource
        {
            string url = GetApiEndpointString<T>();
            return GetAsync<NamedApiResourceList<T>>(
                AddPaginationParamsToUrl(url),
                cancellationToken
            );
        }

        public Task<NamedApiResourceList<T>> GetNamedResourcePageAsync<T>(
            int limit,
            int offset,
            CancellationToken cancellationToken = default
        )
            where T : NamedApiResource
        {
            string url = GetApiEndpointString<T>();
            return GetAsync<NamedApiResourceList<T>>(
                AddPaginationParamsToUrl(url, limit, offset),
                cancellationToken
            );
        }

        public async IAsyncEnumerable<NamedApiResource<T>> GetAllNamedResourcesAsync<T>(
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
            where T : NamedApiResource
        {
            string url = GetApiEndpointString<T>();
            bool isLastPage;

            do
            {
                var page = await GetAsync<NamedApiResourceList<T>>(url, cancellationToken);
                foreach (var resource in page?.Results ?? Enumerable.Empty<NamedApiResource<T>>())
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    yield return resource;
                }

                isLastPage = page?.Next is null;
                if (!isLastPage)
                {
                    url = page!.Next!;
                }
            } while (!cancellationToken.IsCancellationRequested && !isLastPage);
        }

        public Task<ApiResourceList<T>> GetApiResourcePageAsync<T>(
            CancellationToken cancellationToken = default
        )
            where T : ApiResource
        {
            string url = GetApiEndpointString<T>();
            return GetAsync<ApiResourceList<T>>(AddPaginationParamsToUrl(url), cancellationToken);
        }

        public Task<ApiResourceList<T>> GetApiResourcePageAsync<T>(
            int limit,
            int offset,
            CancellationToken cancellationToken = default
        )
            where T : ApiResource
        {
            string url = GetApiEndpointString<T>();
            return GetAsync<ApiResourceList<T>>(
                AddPaginationParamsToUrl(url, limit, offset),
                cancellationToken
            );
        }

        public async IAsyncEnumerable<ApiResource<T>> GetAllApiResourcesAsync<T>(
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
            where T : ApiResource
        {
            string url = GetApiEndpointString<T>();
            bool isLastPage;

            do
            {
                var page = await GetAsync<ApiResourceList<T>>(url, cancellationToken);
                foreach (var resource in page?.Results ?? Enumerable.Empty<ApiResource<T>>())
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    yield return resource;
                }

                isLastPage = page?.Next is null;
                if (!isLastPage)
                {
                    url = page!.Next!;
                }
            } while (!cancellationToken.IsCancellationRequested && !isLastPage);
        }

        private async Task<T> GetResourcesWithParamsAsync<T>(
            string apiParam,
            CancellationToken cancellationToken
        )
            where T : ResourceBase
        {
            // check for case sensitive API endpoint
            bool isApiEndpointCaseSensitive = IsApiEndpointCaseSensitive<T>();
            string sanitizedApiParam = isApiEndpointCaseSensitive
                ? apiParam
                : apiParam.ToLowerInvariant();
            string apiEndpoint = GetApiEndpointString<T>();

            return await GetAsync<T>($"{apiEndpoint}/{sanitizedApiParam}/", cancellationToken);
        }

        private async Task<T> GetResourceByUrlAsync<T>(
            string url,
            CancellationToken cancellationToken
        )
            where T : ResourceBase
        {
            // need to parse out the id in order to make the request
            // navigation urls always use the id of the resource
            string trimmedUrl = url.TrimEnd('/');
            string resourceId = trimmedUrl.Substring(trimmedUrl.LastIndexOf('/') + 1);

            if (!int.TryParse(resourceId, out int id))
            {
                // not sure what to do here...
                throw new NotSupportedException(
                    $"Navigation url '{url}' is in an unexpected format"
                );
            }

            return await GetResourcesWithParamsAsync<T>(resourceId, cancellationToken);
        }

        private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await _client.SendAsync(
                request,
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();
            var result = DeserializeStream<T>(await response.Content.ReadAsStreamAsync());
            return result ?? throw new InvalidOperationException("Failed to deserialize response");
        }

        private T? DeserializeStream<T>(System.IO.Stream stream)
        {
            return JsonSerializer.Deserialize<T>(
                stream,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }
            );
        }

        private static string AddPaginationParamsToUrl(
            string uri,
            int? limit = null,
            int? offset = null
        )
        {
            var queryParameters = new Dictionary<string, string?>();

            if (limit.HasValue)
            {
                queryParameters.Add(nameof(limit), limit.Value.ToString());
            }

            if (offset.HasValue)
            {
                queryParameters.Add(nameof(offset), offset.Value.ToString());
            }

            return AddQueryString(uri, queryParameters);
        }

        private static string GetApiEndpointString<T>()
        {
            PropertyInfo? propertyInfo = typeof(T).GetProperty(
                "ApiEndpoint",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return propertyInfo?.GetValue(null)?.ToString()
                ?? throw new InvalidOperationException(
                    $"ApiEndpoint property not found for type {typeof(T).Name}"
                );
        }

        private static bool IsApiEndpointCaseSensitive<T>()
        {
            PropertyInfo? propertyInfo = typeof(T).GetProperty(
                "IsApiEndpointCaseSensitive",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return propertyInfo?.GetValue(null) as bool? ?? false;
        }

        private static string AddQueryString(string uri, IDictionary<string, string?> queryString)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            return AddQueryString(uri, (IEnumerable<KeyValuePair<string, string?>>)queryString);
        }

        private static string AddQueryString(
            string uri,
            IEnumerable<KeyValuePair<string, string?>> queryString
        )
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            var anchorIndex = uri.IndexOf('#');
            var uriToBeAppended = uri;
            var anchorText = "";
            // If there is an anchor, then the query string must be inserted before its first occurence.
            if (anchorIndex != -1)
            {
                anchorText = uri.Substring(anchorIndex);
                uriToBeAppended = uri.Substring(0, anchorIndex);
            }

            var queryIndex = uriToBeAppended.IndexOf('?');
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uriToBeAppended);
            foreach (var parameter in queryString)
            {
                if (parameter.Value == null)
                {
                    continue;
                }

                sb.Append(hasQuery ? '&' : '?');
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                sb.Append('=');
                sb.Append(UrlEncoder.Default.Encode(parameter.Value));
                hasQuery = true;
            }

            sb.Append(anchorText);
            return sb.ToString();
        }
    }
}
