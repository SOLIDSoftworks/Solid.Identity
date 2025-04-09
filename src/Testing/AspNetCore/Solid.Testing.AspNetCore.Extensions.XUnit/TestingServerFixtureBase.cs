using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public abstract class TestingServerFixtureBase 
    {
        private readonly ConcurrentDictionary<string, ITestOutputHelper> _helpers;
        private readonly AsyncLocal<string> _localTraceId = new AsyncLocal<string>();
        private static readonly string SpanId = ActivitySpanId.CreateRandom().ToHexString();
        public string TraceId => _localTraceId.Value;
        public string TraceParent => _localTraceId.Value != null ? $"00-{_localTraceId.Value}-{SpanId}-00" : null;
        protected virtual string OutputIdHeaderName => "x-output-id";
        
        [Obsolete]
        protected Guid OutputId => Guid.TryParse(_localTraceId.Value, out var guid) ? guid : Guid.Empty;
        
        protected TestingServerFixtureBase()
        {
            _helpers = new ConcurrentDictionary<string, ITestOutputHelper>();
        }

        public void SetOutput(ITestOutputHelper output)
        {
            var traceId = ActivityTraceId.CreateRandom().ToHexString();
            _localTraceId.Value = traceId;
            _helpers.AddOrUpdate(traceId, output, (_, __) => output);
        }

        public bool WriteLine(string message)
        {
            if (_helpers.TryGetValue(_localTraceId.Value, out var helper))
            {
                helper.WriteLine(message);
                return true;
            }

            return false;
        }

        protected void ConfigureRequiredServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.Configure<ChannelLoggerOptions>(options =>
            {
                options.OnCreatingLogMessage = (provider, context) =>
                {
                    var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                    var http = accessor.HttpContext;
                    
                    if(http == null)
                        context.Properties.Add("background", true);
                    
                    if (http?.Request.Headers.TryGetValue(OutputIdHeaderName, out var id) == true)
                    {
                        context.Properties.Add("id", id.ToString());
                        return;
                    }

                    var activity = Activity.Current;
                    if (activity == null) return;
                    context.Properties.Add("id", activity.TraceId.ToHexString());
                };
            });
        }

        protected void ConfigureRequiredTestingServices(IServiceCollection services)
        {
            services
                .ConfigureSolidHttp(builder =>
                {
                    builder.Configure(options =>
                    {
                        options.OnRequestCreated(ConfigureRequest);
                        options.OnHttpRequest(LogRequestStartAsync);
                        options.OnHttpResponse(LogRequestEndAsync);
                    });
                })
            ;
        }

        protected virtual void ConfigureRequest(ISolidHttpRequest request)
        {
            var traceParent = TraceParent;
            if (traceParent != null)
                request.OnHttpRequest(http =>
                {
                    if (http.Headers.TryGetValues(HeaderNames.TraceParent, out var values))
                    {
                        var value = values.First();
                        var match = Regex.Match(value, "^00-([^-]*)-[^-]*-0[01]$");
                        if (!match.Success)
                        {
                            http.Headers.Remove(HeaderNames.TraceParent);
                        }
                        else
                        {
                            var traceId = match.Groups[1].Value;
                            OverrideTraceId(traceId);
                            return;
                        }
                    }
                    http.Headers.Add(HeaderNames.TraceParent, TraceParent);
                });
        }

        public void OverrideTraceId(ActivityTraceId traceId)
            => OverrideTraceId(traceId.ToHexString());
        
        public void OverrideTraceId(string traceId)
        {
            var previous = _localTraceId.Value;
            _localTraceId.Value = traceId;
            if (_helpers.TryGetValue(previous, out var helper))
                _helpers.AddOrUpdate(traceId, helper, (key, _) => helper);
        }

        protected virtual ValueTask LogRequestStartAsync(IServiceProvider services, HttpRequestMessage request)
        {
            if (_background)
            {
                WriteLine("------------------ /background_task ------------------");
                _background = false;
            }
            WriteLine("------------------- http_request ------------------");
            return new ValueTask();
        }

        protected virtual async ValueTask LogRequestEndAsync(IServiceProvider services, HttpResponseMessage response)
        {
            var channel = services.GetRequiredService<LogMessageChannel>();
            var maxWait = TimeSpan.FromMilliseconds(500);
            var maxWaitAfterCompletion = TimeSpan.FromMilliseconds(100);
            var interval = TimeSpan.FromMilliseconds(10);
            var wait = TimeSpan.Zero;
            var waitAfterCompletion = TimeSpan.Zero;
            while (channel.MessagesWaiting)
            {
                await Task.Delay(interval);
                if (channel.Completed)
                    waitAfterCompletion = waitAfterCompletion.Add(interval);
                wait = wait.Add(interval);
                if (wait > maxWait) break;
                if (waitAfterCompletion > maxWaitAfterCompletion) break;
            }

            WriteLine("------------------ /http_request ------------------");
        }

        private bool _background = false;
        protected virtual void ConfigureAspNetCoreHost(AspNetCoreHostOptions options)
        {
            options.OnLogMessage = context =>
            {
                if (context == null) return;
                if (!context.Properties.TryGetValue("id", out var idObj)) return;
                if (!(idObj is string id)) return;
                if (Guid.TryParse(id, out var guid))
                    id = guid.ToString("N"); 
                if (!_helpers.TryGetValue(id, out var helper)) return;
                
                if (context.Properties.TryGetValue("background", out var bgObj) && bgObj is true)
                {
                    if(!_background)
                        WriteLine("----------------- background_task ----------------");
                    _background = true;
                }
                else if (_background)
                {
                    WriteLine("----------------- /background_task ----------------");
                    _background = false;
                }

                helper.WriteLine(context.Message);
            };
        }

        internal void ConfigureAppConfiguration(IConfigurationBuilder builder, Action<IInMemoryConfigurationBuilderRoot> configure, out InMemoryConfigurationSource source)
        {
            var inMemoryConfigurationBuilder = new InMemoryConfigurationBuilder();
            configure(inMemoryConfigurationBuilder);
            source = inMemoryConfigurationBuilder.Build();
            builder.Sources.Add(source);
        }
    }
}
