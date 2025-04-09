using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCoreApplication.Services
{
    public class BackgroundRunner : BackgroundService
    {
        private readonly ILogger<BackgroundRunner> _logger;
        private readonly ConcurrentBag<BackgroundTask> _tasks = new ConcurrentBag<BackgroundTask>();

        public BackgroundRunner(ILogger<BackgroundRunner> logger)
        {
            _logger = logger;
        }

        public void AddBackgroundTask(BackgroundTask task)
            => _tasks.Add(task);

        public bool IsComplete(string id)
            => _tasks.FirstOrDefault(t => t.Id == id)?.Complete == true;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var values = _tasks.ToArray();
                foreach (var task in values.Where(t => !t.Complete))
                {
                    var activity = new Activity(task.Id);
                    activity.SetParentId(task.Traceparent);
                    activity.Start();
                    _logger.LogInformation("Handling task '{id}'", task.Id);
                    if (task.CompleteAt < DateTime.UtcNow)
                    {
                        _logger.LogInformation("Task '{id}' has completed", task.Id);
                        task.Complete = true;
                    }

                    activity.Stop();
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public class BackgroundTask
    {
        public string Id { get; set; }
        public bool Complete { get; set; }
        public string Traceparent { get; set; }
        public DateTime CompleteAt { get; set; }
    }
}