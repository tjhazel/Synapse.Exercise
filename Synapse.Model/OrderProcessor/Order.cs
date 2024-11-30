namespace Synapse.Model.OrderProcessor;

public class Order
{
   public required string OrderId { get; set; }
   public IEnumerable<Item> Items { get; set; } = [];
}
