using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Solid.Extensions.AspNetCore.Soap.Channels;
using Solid.Extensions.AspNetCore.Soap.Logging;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Solid.Extensions.AspNetCore.Soap.Middleware
{
    internal class SoapRequestMiddleware<TService> : IDisposable
    {
        private MessageVersion _version;
        private SoapServiceOptions _options;
        private IDisposable _optionsChangeToken;
        private ILogger _logger;
        private RequestDelegate _next;

        public SoapRequestMiddleware(
            MessageVersion version,
            IOptionsMonitor<SoapServiceOptions> monitor, 
            ILoggerFactory loggerFactory, 
            RequestDelegate next)
        {
            var type = typeof(TService);
            _version = version;
            _options = monitor.Get(type.FullName);
            _optionsChangeToken = monitor.OnChange((options, name) =>
            {
                if (name == type.FullName)
                    _options = options;
            });
            _logger = loggerFactory.CreateLogger("Solid.Extensions.AspNetCore.Soap.Middleware.SoapRequestMiddleware");
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var activity = Tracing.Soap.StartActivity($"SoapRequestMiddleware<{typeof(TService).Name}>.{nameof(InvokeAsync)}");
            using (_logger.BeginScope($"Soap contract namespace: {_options.Namespace}"))
            using (_logger.BeginScope($"Soap contract name: {_options.Name}"))
            {
                await context.Request.EnableRewindAsync();
                await using (context.Response.Buffer())
                {
                    using (var reader = XmlReader.Create(context.Request.Body))
                    {
                        var request = Message.CreateMessage(reader, _options.MaxSizeOfHeaders, _version);

                        if (request.Headers.Action == null)
                            request.Headers.Action = context.Request.Headers["SOAPAction"];

                        LoggerMessages.LogIncomingRequest(_logger, ref request);

                        var soap = new SoapContext<TService>(context, request, _version, _options);
                        context.SetSoapContext(soap);
                        try
                        {
                            await Next(context);
                        }
                        catch (TargetInvocationException ex) when (ex.InnerException is FaultException fault)
                        {
                            HandleFaultException(fault, soap);
                        }
                        catch (FaultException ex)
                        {
                            HandleFaultException(ex, soap);
                        }

                        if (soap?.Response == null) return; // throw exception?
                        if (context.Response.Body.Length > 0) return; // response manually written

                        var response = soap.Response;

                        if (response.IsFault)
                            context.Response.StatusCode = 500;
                        else
                            context.Response.StatusCode = 200;

                        context.Response.ContentType = soap.ContentType;

                        LoggerMessages.LogOutgoingResponse(_logger, ref response);

                        using (var writer = XmlWriter.Create(context.Response.Body, new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = new UTF8Encoding(false) }))
                            response.WriteMessage(writer);

                        context.Response.RegisterForDispose(request);
                        context.Response.RegisterForDispose(response);
                    }
                }
            }
        }

        private void HandleFaultException(FaultException ex, SoapContext<TService> context)
        {
            _logger.LogWarning(ex, $"Fault exception: {ex.Message}");
            var messageFault = ex.CreateMessageFault();
            context.Response = FaultMessage.CreateFaultMessage(context.MessageVersion, messageFault, context.Request.Headers.Action);
        }

        private async Task Next(HttpContext context)
        {
            var soap = context.GetSoapContext();
            try
            {
                _logger.LogTrace("Invoking next middleware in pipeline.");
                await _next(context);
            }
            catch (TargetInvocationException ex) when (!(ex.InnerException is FaultException))
            {
                if (ex.InnerException is FaultException) throw;

                _logger.LogError(ex.InnerException ?? ex, "Unhandled server exception");
                throw CreateFaultException(soap, ex.InnerException ?? ex);
            }
            catch (Exception ex)
            {
                if (ex is FaultException) throw;
                if (ex is TargetInvocationException tiex && tiex.InnerException != null && tiex.InnerException is FaultException) throw;

                _logger.LogError(ex, "Unhandled server exception");
                throw CreateFaultException(soap, ex);
            }
        }

        private FaultException CreateFaultException(SoapContext context, Exception exception)
        {
            var version = context.MessageVersion;
            var code = version.CreateFaultCode();
            if (context.Options.IncludeExceptionDetailInFaults)
            {
                var detail = new ExceptionDetail(exception);
                return new FaultException<ExceptionDetail>(detail, version.CreateFaultReason(exception.Message));
            }

            return new FaultException(version.CreateFaultReason(), code, context.Request.Headers.Action);
        }

        public void Dispose() => _optionsChangeToken?.Dispose();
    }
}
