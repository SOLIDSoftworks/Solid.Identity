using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public interface IInMemoryConfigurationBuilderRoot
    {
        IInMemoryConfigurationBuilderRoot Add(string key, bool value);
        IInMemoryConfigurationBuilderRoot Add<T>(string key, T value);
        IInMemoryConfigurationBuilderRoot Add<T>(string key, T value, Func<T, string> convert);
        IInMemoryConfigurationBuilderRoot Add(string key, Action<IInMemoryConfigurationBuilder> addChildren);
        IInMemoryConfigurationBuilderRoot SetLogLevel<TCategoryName>(LogLevel level);
        IInMemoryConfigurationBuilderRoot SetLogLevel(string categoryName, LogLevel level);
        IInMemoryConfigurationBuilderRoot IncludeLoggingScopes(bool includeScopes);
        IInMemoryConfigurationBuilderRoot SetDefaultLogLevel(LogLevel level);
    }
}
