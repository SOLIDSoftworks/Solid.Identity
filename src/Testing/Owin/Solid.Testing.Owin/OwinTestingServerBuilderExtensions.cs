using Solid.Testing.Extensions.Owin.Factories;
using System;

namespace Solid.Http
{
    /// <summary>
    /// The owin testing server builder extensions
    /// </summary>
    public static class OwinTestingServerBuilderExtensions
    {
        /// <summary>
        /// Adds a host factory for asp.net owin
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddOwinHostFactory(this TestingServerBuilder builder)
        {
            return builder.AddHostFactory(new OwinInMemoryHostFactory());
        }
    }
}
