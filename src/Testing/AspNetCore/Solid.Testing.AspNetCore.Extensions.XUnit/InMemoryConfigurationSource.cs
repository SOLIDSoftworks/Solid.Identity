using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    class InMemoryConfigurationSource : IConfigurationSource
    {
        public InMemoryConfigurationSource(IDictionary<string, string> initialData)
        {
            Provider = new InMemoryConfigurationProvider(initialData);
        }
        public InMemoryConfigurationProvider Provider { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => Provider;
    }
}
