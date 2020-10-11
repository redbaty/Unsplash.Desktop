using System;
using Microsoft.Extensions.Logging;
using Console = Colorful.Console;

namespace Unsplash.Desktop
{
    internal class ConsoleLogger : ILogger<Core.Unsplash>
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter.Invoke(state, exception);
            Console.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}