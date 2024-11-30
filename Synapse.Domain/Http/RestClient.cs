using System.Net.Mime;
using System.Text.Json;
using System.Text;
using Synapse.Model.Domain;
using System.Net.Http.Headers;

namespace Synapse.Domain.Http;

/// <summary>
/// Abstraction to wrap httpclient so we can mock http requests.   This class will not be called
/// with the unit test. In a real world scenario, the entry point would be a azure function
/// or some other method of kicking it off.
/// </summary>
/// <param name="_httpClientFactory"></param>
public class RestClient(IHttpClientFactory _httpClientFactory) : IRestClient
{
   private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
   {
      DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
   };

   public async Task<RestResult<T>> SendRequest<T>(HttpMethod method, string requestUrl, object? body = null) where T : class
   {
      var httpClient = _httpClientFactory.CreateClient();

      using HttpRequestMessage request = new (method, requestUrl);

      if (body != null)
      {
         string requestContent = JsonSerializer.Serialize(body, _jsonSerializerOptions);
         request.Content = new StringContent(requestContent, Encoding.UTF8, MediaTypeNames.Application.Json);
      }

      HttpResponseMessage response = await httpClient.SendAsync(request);
      //Would expect to add some headers
      //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ###);

      var responseContent = await response.Content.ReadAsStringAsync();

      RestResult<T> result = new()
      {
         IsSuccessStatusCode = response.IsSuccessStatusCode
      };

      // string is a string and does not Deserialize well 
      if (typeof(T) == typeof(string))
      {
         result.Result = responseContent as T;
      }
      else if (string.IsNullOrWhiteSpace(responseContent))
      {
         result.Result = default(T);
      }
      else
      {
         result.Result = JsonSerializer.Deserialize<T>(responseContent);
      }
      return result;
   }
}
