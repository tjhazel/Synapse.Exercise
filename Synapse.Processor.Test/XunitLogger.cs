using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Synapse.Processor.Test;

/// <summary>
/// Logger to allow unit tests to capture ILogger results. Borrowed 
/// from https://stackoverflow.com/a/47713709/2661476
/// </summary>
/// <typeparam name="T"></typeparam>
public class XunitLogger<T> : ILogger<T>, IDisposable
{
   private ITestOutputHelper _output;

   public XunitLogger(ITestOutputHelper output)
   {
      _output = output;
   }
   public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
   {
      _output.WriteLine(state.ToString());
   }

   public bool IsEnabled(LogLevel logLevel)
   {
      return true;
   }

   public IDisposable BeginScope<TState>(TState state)
   {
      return this;
   }

   public void Dispose()
   {
   }
}