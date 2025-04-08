using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Solid.Extensions.AspNetCore.Soap.Builder
{
    internal class SoapApplicationBuilder<TService> : ISoapApplicationBuilder, IApplicationBuilder
    {
        private IApplicationBuilder _inner;

        public SoapApplicationBuilder(PathString path, IApplicationBuilder inner)
        {
            Path = path;
            _inner = inner;
        }

        public PathString Path { get; }

        public Type Contract => typeof(TService);

        public SoapServiceOptions Options => ApplicationServices.GetService<IOptionsMonitor<SoapServiceOptions>>().Get(Contract.FullName);

        public IServiceProvider ApplicationServices { get => _inner.ApplicationServices; set => _inner.ApplicationServices = value; }

        public IFeatureCollection ServerFeatures => _inner.ServerFeatures;

        public IDictionary<string, object> Properties => _inner.Properties;

        public RequestDelegate Build() => _inner.Build();

        public IApplicationBuilder New() => _inner.New();

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) => _inner.Use(middleware);
    }
}
