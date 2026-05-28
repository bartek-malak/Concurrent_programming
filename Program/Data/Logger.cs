using System.Collections.Concurrent;
using System.Text.Json;

namespace Data
{
    internal class Logger : IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();

        private readonly Task _loggingTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly string _filePath;

        public Logger(string filePath = "diagnostics.json")
        {
            _filePath = filePath;

            // Uruchamiamy jednego "pracownika" w tle, który będzie zapisywał plik
            _loggingTask = Task.Run(LogLoop);
        }

        // Metoda dla kul (Producentów)
        public void Log(object data)
        {
            // Opcje JSON
            var options = new JsonSerializerOptions { WriteIndented = false };

            // Zamieniamy obiekt kuli na tekst JSON i wrzucamy do kosza
            string jsonString = JsonSerializer.Serialize(data, options);
            _logQueue.Enqueue($"[{DateTime.Now:HH:mm:ss.fff}] {jsonString}");
        }

        // Pętla naszego konsumenta
        private async Task LogLoop()
        {
            // Otwieramy plik do dopisywania (Append)
            using StreamWriter writer = new StreamWriter(_filePath, append: true);

            // Dopóki ktoś nie wywoła Dispose()
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_logQueue.TryDequeue(out string log))
                {
                    // Zapisujemy na dysk
                    await writer.WriteLineAsync(log);
                }
                else
                {
                    await Task.Delay(10);
                }
            }

            // Aplikacja się zamyka. Zapisujemy to, co zostało w koszu!
            while (_logQueue.TryDequeue(out string log))
            {
                await writer.WriteLineAsync(log);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _loggingTask.Wait();
            _cts.Dispose();
        }
    }
}