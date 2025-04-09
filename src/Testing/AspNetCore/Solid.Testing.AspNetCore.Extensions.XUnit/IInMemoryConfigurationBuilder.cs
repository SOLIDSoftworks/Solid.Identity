using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public interface IInMemoryConfigurationBuilder
    {
        IInMemoryConfigurationBuilder Add(string key, bool value);
        IInMemoryConfigurationBuilder Add<T>(string key, T value);
        IInMemoryConfigurationBuilder Add<T>(string key, T value, Func<T, string> convert);
        IInMemoryConfigurationBuilder Add(string key, Action<IInMemoryConfigurationBuilder> addChildren);
    }
}
