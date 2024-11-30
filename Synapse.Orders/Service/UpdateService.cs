using Microsoft.Extensions.Logging;
using Synapse.Domain.Http;
using Synapse.Model.Domain;
using Synapse.Model.OrderProcessor;

namespace Synapse.Processor.Service;

public class UpdateService(ILogger<UpdateService> _logger, 
   IRestClient _restClient)
{
   public async Task SendAlertAndUpdateOrder(Order order)
   {
      const string updateApiUrl = "https://update-api.com/update";
      var response = await _restClient.SendRequest<VoidResult>(HttpMethod.Post, updateApiUrl, order);
      if (response.IsSuccessStatusCode)
      {
         _logger.LogInformation($"Updated order sent for processing: OrderId {order.OrderId}");
      }
      else
      {
         _logger.LogError($"Failed to send updated order for processing: OrderId {order.OrderId}");
      }
   }
}
