using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    internal class InMemoryConfigurationBuilder : IInMemoryConfigurationBuilderRoot, IInMemoryConfigurationBuilder
    {
        private string _delimiter;
        private string _prefix;
        internal Dictionary<string, string> InMemoryConfiguration { get; }

        public InMemoryConfigurationBuilder()
            : this(string.Empty, new Dictionary<string, string>())
        {

        }

        private InMemoryConfigurationBuilder(string prefix, Dictionary<string, string> configuration)
        {
            _delimiter = ConfigurationPath.KeyDelimiter;
            _prefix = prefix;
            InMemoryConfiguration = configuration;
        }

        internal InMemoryConfigurationSource Build() => new InMemoryConfigurationSource(InMemoryConfiguration);

        private InMemoryConfigurationBuilder Add(string key, bool value) => Add<bool>(key, value, t => t.ToString().ToLower());
        private InMemoryConfigurationBuilder Add<T>(string key, T value) => Add<T>(key, value, t => t?.ToString());

        private InMemoryConfigurationBuilder Add<T>(string key, T value, Func<T, string> convert) => Add(key, convert(value));

        private InMemoryConfigurationBuilder Add(string key, Action<InMemoryConfigurationBuilder> addChildren)
        {
            var builder = new InMemoryConfigurationBuilder($"{_prefix}{key}{_delimiter}", InMemoryConfiguration);
            addChildren(builder);
            return this;
        }

        private InMemoryConfigurationBuilder SetLogLevel<TCategoryName>(LogLevel level) => SetLogLevel(typeof(TCategoryName).FullName, level);
        private InMemoryConfigurationBuilder SetDefaultLogLevel(LogLevel level) => SetLogLevel("Default", level);

        private InMemoryConfigurationBuilder SetLogLevel(string categoryName, LogLevel level)
        {
            var path = new[]
            {
                "Logging",
                "LogLevel",
                categoryName
            };
            var key = string.Join(_delimiter, path);
            Set(key, level.ToString());
            return this;
        }

        private IInMemoryConfigurationBuilderRoot IncludeScopes(bool includeScopes)
        {
            var path = new[]
            {
                "Logging",
                "IncludeScopes"
            };
            var key = string.Join(_delimiter, path);
            Set(key, includeScopes.ToString().ToLower());
            return this;
        }

        private InMemoryConfigurationBuilder Add(string key, string value) => Set($"{_prefix}{key}", value);

        private InMemoryConfigurationBuilder Set(string path, string value)
        {
            InMemoryConfiguration[path] = value;
            return this;
        }

        IInMemoryConfigurationBuilder IInMemoryConfigurationBuilder.Add(string key, bool value) => Add(key, value);

        IInMemoryConfigurationBuilder IInMemoryConfigurationBuilder.Add<T>(string key, T value) => Add(key, value);

        IInMemoryConfigurationBuilder IInMemoryConfigurationBuilder.Add<T>(string key, T value, Func<T, string> convert) => Add(key, value, convert);

        IInMemoryConfigurationBuilder IInMemoryConfigurationBuilder.Add(string key, Action<IInMemoryConfigurationBuilder> addChildren) => Add(key, b => addChildren(b));

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.Add(string key, bool value) => Add(key, value);

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.Add<T>(string key, T value) => Add(key, value);

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.Add<T>(string key, T value, Func<T, string> convert) => Add(key, value, convert);

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.Add(string key, Action<IInMemoryConfigurationBuilder> addChildren) => Add(key, b => addChildren(b));

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.SetLogLevel<TCategoryName>(LogLevel level) => SetLogLevel<TCategoryName>(level);

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.SetLogLevel(string categoryName, LogLevel level) => SetLogLevel(categoryName, level);

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.IncludeLoggingScopes(bool includeScopes) => IncludeScopes(includeScopes);

        IInMemoryConfigurationBuilderRoot IInMemoryConfigurationBuilderRoot.SetDefaultLogLevel(LogLevel level) => SetDefaultLogLevel(level);
    }
}
