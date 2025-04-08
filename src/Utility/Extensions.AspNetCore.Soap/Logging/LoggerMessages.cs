using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Solid.Extensions.AspNetCore.Soap.Logging
{
    internal static class LoggerMessages
    {
        private const int _bufferSize = 8 * 1024; // TODO: use better way to get this number

        private static readonly LogLevel ShortCirtuitingPiplineLogLevel = LogLevel.Debug;
        private static readonly LogLevel InvokingMiddlewareLogLevel = LogLevel.Debug;
        private static readonly LogLevel IncomingRequestLogLevel = LogLevel.Trace;
        private static readonly LogLevel OutgoingResponseLogLevel = LogLevel.Trace;

        public static void LogShortCircuitingPipeline(ILogger logger)
        {
            if (!logger.IsEnabled(ShortCirtuitingPiplineLogLevel)) return;
            ShortCircuitingPipeline(logger, null);
        }
        public static void LogInvokingMiddleware(ILogger logger, Type type)
        {
            if (!logger.IsEnabled(InvokingMiddlewareLogLevel)) return;
            InvokingMiddleware(logger, type.Name, null);
        }
        public static void LogIncomingRequest(ILogger logger, ref Message message)
        {
            if (!logger.IsEnabled(IncomingRequestLogLevel)) return;

            using var buffer = message.CreateBufferedCopy(_bufferSize);
            IncomingRequest(logger, buffer.ReadAll(), null);
            message = buffer.CreateMessage();
        }
        public static void LogOutgoingResponse(ILogger logger, ref Message message)
        {
            if (!logger.IsEnabled(OutgoingResponseLogLevel)) return;
            if (message.IsEmpty) return;
            
            using var buffer = message.CreateBufferedCopy(_bufferSize);
            OutgoingResponse(logger, buffer.ReadAll(), null);
            message = buffer.CreateMessage();
        }

        private static readonly Action<ILogger, string, Exception> InvokingMiddleware
            = LoggerMessage.Define<string>(InvokingMiddlewareLogLevel, 0, "Invoking middleware '{Name}'.");

        private static readonly Action<ILogger, string, Exception> IncomingRequest
            = LoggerMessage.Define<string>(IncomingRequestLogLevel, 0, "Incoming SOAP request:" + Environment.NewLine + "{Request}");

        private static readonly Action<ILogger, string, Exception> OutgoingResponse
            = LoggerMessage.Define<string>(OutgoingResponseLogLevel, 0, "Outgoing SOAP response:" + Environment.NewLine + "{Response}");

        private static readonly Action<ILogger, Exception> ShortCircuitingPipeline
            = LoggerMessage.Define(ShortCirtuitingPiplineLogLevel, 0, "Response has been created. Short-circuiting pipeline.");
    }
}
