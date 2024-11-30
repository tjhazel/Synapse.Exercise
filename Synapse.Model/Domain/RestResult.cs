namespace Synapse.Model.Domain;

public class RestResult<T> where T : class
{
   public bool IsSuccessStatusCode { get; set; }
   public T? Result { get; set; }
}
