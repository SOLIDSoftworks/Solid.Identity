using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Solid.ServiceModel
{
    internal static class LogMessages
    {
        public static void CreatingChannelWithIssuedToken(ILogger logger, Type type)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            CreatingChannelWithIssuedTokenLoggerMessage(logger, type.Name, null);
        }

        public static void ExtendedClientCredentialsNotFound(ILogger logger)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            ClientCredentialsNotFoundLoggerMessage(logger, nameof(ExtendedClientCredentials), null);
        }

        public static void CreatingSecurityTokenManager(ILogger logger)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            CreatingSecurityTokenManagerLoggerMessage(logger, null);
        }

        public static void CreatingIssuedTokenProvider(ILogger logger)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            CreatingIssuedTokenProviderLoggerMessage(logger, null);
        }

        public static void CreatingDefaultSecurityTokenProvider(ILogger logger)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            CreatingDefaultSecurityTokenProviderLoggerMessage(logger, null);
        }

        public static void AddingSecurityTokenToRequest(ILogger logger, SecurityToken token)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Information)) return;

            AddingSecurityTokenToRequestLoggerMessage(logger, token.GetType().Name, null);
        }

        public static void FoundSecurityTokenHandler(ILogger logger, SecurityTokenHandler handler)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            FoundSecurityTokenHandlerLoggerMessage(logger, handler.GetType().Name, null);
        }

        public static void SoapRequest(ILogger logger, ref Message request)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Trace)) return;

            SoapRequestLoggerMessage(logger, GetMessageXml(request, out request), null);
        }

        public static void SoapResponse(ILogger logger, ref Message reply)
        {
            if (logger == null) return;
            if (!logger.IsEnabled(LogLevel.Trace)) return;

            SoapResponseLoggerMessage(logger, GetMessageXml(reply, out reply), null);
        }

        public static IDisposable BeginSoapRequestScope(ILogger logger, Message message)
        {
            if (logger == null) return null;

            return BeginSoapRequestScopeLoggerMessage(logger, message.Headers?.Action);
        }

        static Action<ILogger, string, Exception> AddingSecurityTokenToRequestLoggerMessage
            = LoggerMessage.Define<string>(LogLevel.Information, 0, "Adding {TokenType} to SOAP request.");

        static Action<ILogger, string, Exception> FoundSecurityTokenHandlerLoggerMessage
            = LoggerMessage.Define<string>(LogLevel.Debug, 0, "Found security token handler '{HandlerType}' to write security token to SOAP request.");

        static Action<ILogger, Exception> CreatingSecurityTokenManagerLoggerMessage
            = LoggerMessage.Define(LogLevel.Debug, 0, "Creating security token manager.");

        static Action<ILogger, Exception> CreatingIssuedTokenProviderLoggerMessage
            = LoggerMessage.Define(LogLevel.Debug, 0, "Creating issued token provider.");

        static Action<ILogger, Exception> CreatingDefaultSecurityTokenProviderLoggerMessage
            = LoggerMessage.Define(LogLevel.Debug, 0, "Creating default security token provider.");

        static Action<ILogger, string, Exception> ClientCredentialsNotFoundLoggerMessage
            = LoggerMessage.Define<string>(LogLevel.Debug, 0, "{ClientCredentialsType} not found.");

        static Action<ILogger, string, Exception> CreatingChannelWithIssuedTokenLoggerMessage
            = LoggerMessage.Define<string>(LogLevel.Debug, 0, "Creating channel for {ContractType} with issued token.");

        static Action<ILogger, string, Exception> SoapRequestLoggerMessage
            = LoggerMessage.Define<string>(LogLevel.Trace, 0, "SOAP request:" + Environment.NewLine + "{RequestXml}");

        static Action<ILogger, string, Exception> SoapResponseLoggerMessage
            = LoggerMessage.Define<string>(LogLevel.Trace, 0, "SOAP response:" + Environment.NewLine + "{ResponseXml}");

        static Func<ILogger, string, IDisposable> BeginSoapRequestScopeLoggerMessage
            = LoggerMessage.DefineScope<string>("SOAP request action: {Action}");

        static string GetMessageXml(Message message, out Message bufferedCopy)
        {
            var buffer = message.CreateBufferedCopy(int.MaxValue);
            var copy = buffer.CreateMessage();
            using(var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false, Indent = true, OmitXmlDeclaration = true }))
                    copy.WriteMessage(writer);
                bufferedCopy = buffer.CreateMessage();

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
