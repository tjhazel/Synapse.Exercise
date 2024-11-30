using Microsoft.Extensions.Logging;
using Synapse.Domain.Http;
using Synapse.Model.Domain;
using Synapse.Model.OrderProcessor;

namespace Synapse.Processor.Service;

public class AlertService(ILogger<AlertService> _logger, 
   IRestClient _restClient)
{
   public async Task SendAlertMessage(Item item, string orderId)
   {
      AlertData alertData = new()
      {
         Message = $"""
                     Alert for delivered item: Order {orderId}, Item: {item.Description}, 
                     Delivery Notifications: {item.DeliveryNotification}
                     """
      };

      const string alertApiUrl = "https://alert-api.com/alerts";
      var response = await _restClient.SendRequest<VoidResult>(HttpMethod.Post, alertApiUrl, alertData);
      if (response.IsSuccessStatusCode)
      {
         _logger.LogInformation($"Alert sent for delivered item: {item.Description}");
      }
      else
      {
         _logger.LogError($"Failed to send alert for delivered item: {item.Description}");
      }
   }
}
