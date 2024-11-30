using Microsoft.Extensions.Logging;
using Synapse.Domain.Http;
using Synapse.Model.OrderProcessor;

namespace Synapse.Processor.Service;

public class OrderService(ILogger<OrderService> _logger, 
   IRestClient _restClient)
{
   public async Task<IEnumerable<Order>> FetchMedicalEquipmentOrders()
   {
      const string ordersApiUrl = "https://orders-api.com/orders";
      var response = await _restClient.SendRequest<IEnumerable<Order>>(HttpMethod.Get, ordersApiUrl);
      if (!response.IsSuccessStatusCode)
      {
         _logger.LogInformation("Failed to fetch orders from API.");
      }    

      return response.Result ?? Enumerable.Empty<Order>();
   }
}
