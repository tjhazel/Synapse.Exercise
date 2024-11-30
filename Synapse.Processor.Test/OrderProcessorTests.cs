using Microsoft.Extensions.Logging;
using Moq;
using Synapse.Domain.Http;
using Synapse.Model.Domain;
using Synapse.Model.OrderProcessor;
using Synapse.Processor.Service;
using Xunit.Abstractions;

namespace Synapse.Processor.Test
{
   public class OrderProcessorTests(ITestOutputHelper _output)
   {
      /// <summary>
      /// This is the only test in place to exercise the code.  The basic idea is to 
      /// process all the orders using a mock to simulate the http calls. 
      /// </summary>
      /// <returns></returns>
      [Fact]
      public async Task OrderProcessorTests_Success()
      {
         //arrange
         var testData = GetTestData();
         var expectedDeliveryCount = testData.Sum(y => y.Items.Count(x => x.IsItemDelivered));
         var startingDeliveryCount = testData.Sum(y => y.Items.Count(x => x.DeliveryNotification == 1));

         OrderProcessor orderProcessor = GetOrderProcessor(testData);

         //act
         await orderProcessor.Process();

         //assert
         var actualDeliveryCount = testData.Sum(y => y.Items.Count(x => x.DeliveryNotification == 1));

         Assert.True(startingDeliveryCount == 0, "Starting delivery count should be zero");
         Assert.True(expectedDeliveryCount == actualDeliveryCount, "Expected delivery count should match the actual delivery count");
      }

      private OrderProcessor GetOrderProcessor(IEnumerable<Order> testData)
      {
         //arrange
         RestResult<VoidResult> voidRestResult = new() { Result = new(), IsSuccessStatusCode = true };
         RestResult<IEnumerable<Order>> orderRestResult = new() { Result = testData, IsSuccessStatusCode = true };

         Mock<IRestClient> mockRestClient = new();
         mockRestClient
             .Setup(y => y.SendRequest<VoidResult>(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<object?>()))
             .ReturnsAsync(voidRestResult);
         mockRestClient
            .Setup(y => y.SendRequest<IEnumerable<Order>>(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(orderRestResult);

         XunitLogger<AlertService> alertServiceLogger = new(_output);
         AlertService alertService = new(alertServiceLogger, mockRestClient.Object);

         XunitLogger<OrderService> orderServiceLogger = new(_output);
         OrderService orderService = new(orderServiceLogger, mockRestClient.Object);

         XunitLogger<UpdateService> updateServiceLogger = new(_output);
         UpdateService updateService = new(updateServiceLogger, mockRestClient.Object);

         Mock<ILogger<OrderProcessor>> mockOrderProcessorLogger = new();
         OrderProcessor orderProcessor = new(mockOrderProcessorLogger.Object,
            alertService,
            orderService,
            updateService);

         return orderProcessor;
      }

      private IEnumerable<Order> GetTestData()
      {
         return new List<Order>
         {
            new ()
            {
               OrderId = Guid.NewGuid().ToString(),
               Items = [
                  new Item{ Status="Delivered", Description="Blood sugar meter" },
                  new Item{ Status="Delivered", Description="Blood sugar test strip" },
              ]
            },
            new ()
            {
               OrderId = Guid.NewGuid().ToString(),
               Items = [
                  new Item{ Status="Delivered", Description="Cane" },
                  new Item{ Status="New", Description="Crutches" },
                  new Item{ Status="Delivered", Description="Hospital beds" },
                  new Item{ Status="Delivered", Description="Patient lifts " }
              ]
            },
            new ()
            {
               OrderId = Guid.NewGuid().ToString(),
               Items = [
                  new Item{ Status="Delivered", Description="Traction equipment" },
                  new Item{ Status="New", Description="Walkers" },
                  new Item{ Status="Delivered", Description="Wheelchairs & scooters " }
              ]
            }
         };
      }
   }
}