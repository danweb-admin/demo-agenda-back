using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Utils
{
    public class CustomLogger : ILogger
    {
        readonly string loggerName;
        readonly CustomLoggerProviderConfiguration config;
        private static readonly object _lock = new(); 

        public CustomLogger(string name, CustomLoggerProviderConfiguration _config)
        {
            loggerName = name;
            config = _config;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = $"{DateTime.Now} - {logLevel.ToString()}: {eventId.Id} - {formatter(state,exception)}";
            WriteTextInFile(message);
        }

        private void WriteTextInFile(string message)
        {
            try
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                var path = Environment.GetEnvironmentVariable("LogPath");

                // 🔧 Se a variável não estiver definida, usa /app/logs como padrão
                if (string.IsNullOrEmpty(path))
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

                // ✅ Garante que o diretório existe
                Directory.CreateDirectory(path);

                string fullPath = Path.Combine(path, $"log{today}.txt");

                // 🔒 Evita conflito de escrita simultânea
                lock (_lock)
                {
                    using (var stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao escrever no log: {e}");
            }
        }
    }
}
