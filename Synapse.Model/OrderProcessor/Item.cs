namespace Synapse.Model.OrderProcessor;

public class Item
{
   public required string Status { get; set; }
   public required string Description { get; set; }

   private int _deliveryNotification = 0;
   public int DeliveryNotification
   {
      get
      {
         return _deliveryNotification;
      }
   }

   public void IncrementDeliveryNotification()
   {
      Interlocked.Increment(ref _deliveryNotification);
   }

   public bool IsItemDelivered
   {
      get
      {
         return Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase);
      }
   }
}
