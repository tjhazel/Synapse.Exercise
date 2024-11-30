using Synapse.Model.Domain;

namespace Synapse.Domain.Http;

/// <summary>
/// Created IRestClient interface for moq support
/// </summary>
public interface IRestClient
{
   Task<RestResult<T>> SendRequest<T>(HttpMethod method, string requestUrl, object? body = null) where T : class;
}
