using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreApplication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreApplication
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationOptions>(_configuration.GetSection("Options"), o => o.BindNonPublicProperties = false);
            services.AddLogging(logging => logging.ClearProviders());
            services.AddMvc();
            services.AddSingleton<BackgroundRunner>();
            services.AddHostedService(p => p.GetService<BackgroundRunner>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .Use((HttpContext context, Func<Task> next) =>
                {
                    var options = context.RequestServices.GetService<IOptionsSnapshot<LoggerFilterOptions>>().Value;
                    var factory = context.RequestServices.GetService<ILoggerFactory>();
                    var logger = factory.CreateLogger<Startup>();

                    var rule = options.Rules.FirstOrDefault();
                    logger.Log(LogLevel.Trace, rule?.LogLevel?.ToString() ?? "no rule");
                    logger.Log(LogLevel.Debug, rule?.LogLevel?.ToString() ?? "no rule");
                    logger.Log(LogLevel.Information, rule?.LogLevel?.ToString() ?? "no rule");
                    logger.Log(LogLevel.Warning, rule?.LogLevel?.ToString() ?? "no rule");
                    logger.Log(LogLevel.Error, rule?.LogLevel?.ToString() ?? "no rule");
                    logger.Log(LogLevel.Critical, rule?.LogLevel?.ToString() ?? "no rule");

                    return next();
                })
                .Map("/backgroundtasks", builder =>
                {
                    builder.Use(async (HttpContext context, Func<Task> next) =>
                    {
                        var logger = context.RequestServices.GetService<ILoggerFactory>().CreateLogger("BackgroundTaskHandler");
                        var runner = context.RequestServices.GetService<BackgroundRunner>();
                        if (context.Request.Method == "POST")
                        {
                            logger.LogInformation("Creating background task");
                            var task = new BackgroundTask
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                CompleteAt = DateTime.UtcNow.AddSeconds(10),
                                Traceparent = Activity.Current?.Id
                            };
                            runner.AddBackgroundTask(task);
                            context.Response.ContentType = "text/plain";
                            context.Response.StatusCode = 201;
                            await context.Response.WriteAsync(task.Id);
                            return;
                        }
                        if (context.Request.Method == "GET")
                        {
                            var id = ((string)context.Request.Path).Trim('/');
                            logger.LogInformation("Checking status of background task '{id}'", id);
                            if (runner.IsComplete(id))
                            {
                                logger.LogInformation("Background task '{id}' has completed", id);
                                context.Response.ContentType = "text/plain";
                                context.Response.StatusCode = 200;
                                await context.Response.WriteAsync(bool.TrueString);
                            }
                            else
                            {
                                logger.LogInformation("Background task '{id}' has not completed", id);
                                context.Response.ContentType = "text/plain";
                                context.Response.StatusCode = 400;
                                await context.Response.WriteAsync(bool.FalseString);
                            }

                            return;
                        }
                        context.Response.StatusCode = 404;
                    });
                })
                .Use(async (HttpContext context, Func<Task> next) =>
                {
                    var path = ((string)context.Request.Path).Trim('/');
                    var value = null as string;
                    if(path.Equals("options", StringComparison.OrdinalIgnoreCase))
                    {
                        var options = context.RequestServices.GetService<IOptionsMonitor<ApplicationOptions>>();
                        value = options.CurrentValue.Value;
                    }
                    else
                    {
                        var configuration = context.RequestServices.GetService<IConfiguration>();
                        var section = configuration.GetSection(path);
                        value = section.GetValue<string>("Value");
                    }

                    var bytes = Encoding.UTF8.GetBytes(value);
                    context.Response.ContentType = "text/plain";
                    context.Response.ContentLength = value?.Length;
                    context.Response.StatusCode = 200;
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                })
            ;
        }
    }
}
