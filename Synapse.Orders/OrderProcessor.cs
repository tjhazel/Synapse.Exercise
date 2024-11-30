﻿using Microsoft.Extensions.Logging;
using Synapse.Model.OrderProcessor;
using Synapse.Processor.Service;

namespace Synapse.Processor;

//intended to be called from any sort of consumer
public class OrderProcessor(ILogger<OrderProcessor> _logger,
    AlertService _alertService,
    OrderService _orderService,
    UpdateService _updateService)
 {
   public async Task Process()
   {
      _logger.LogInformation("Start of Process");

      //consider producer\consumer for inprocess items or a queue\bus\hub for isolated
      var medicalEquipmentOrders = await _orderService.FetchMedicalEquipmentOrders();

      foreach (var order in medicalEquipmentOrders)
      {
         var updatedOrder = await ProcessOrder(order);
         await _updateService.SendAlertAndUpdateOrder(updatedOrder);
      }

      _logger.LogInformation("Results sent to relevant APIs.");
   }

   private async Task<Order> ProcessOrder(Order order)
   {
      foreach (var item in order.Items)
      {
         if (item.IsItemDelivered)
         {
            await _alertService.SendAlertMessage(item, order.OrderId);
            item.IncrementDeliveryNotification();
         }
      }

      return order;
   }

}