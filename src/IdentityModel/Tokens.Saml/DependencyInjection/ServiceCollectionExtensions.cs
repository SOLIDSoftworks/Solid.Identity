using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.IdentityModel.Tokens.Saml2;
using Solid.IdentityModel.Tokens.Saml2.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSaml2EncryptedSecurityTokenHandler(this IServiceCollection services)
            => services.AddSaml2EncryptedSecurityTokenHandler<Saml2EncryptedSecurityTokenHandler>();

        public static IServiceCollection AddSaml2EncryptedSecurityTokenHandler<THandler>(this IServiceCollection services)
            where THandler : Saml2EncryptedSecurityTokenHandler
        {
            services.TryAddTransient<Saml2SecurityTokenHandler, THandler>();
            return services;
        }

        public static IServiceCollection AddSaml2Serializer<TSerializer>(this IServiceCollection services)
            where TSerializer : Saml2Serializer
        {
            services.RemoveAll<Saml2Serializer>();
            services.AddTransient<Saml2Serializer, TSerializer>();
            return services;
        }

        public static IServiceCollection AddSaml2MetadataSerializer(this IServiceCollection services)
            => services.AddSaml2MetadataSerializer<Saml2MetadataSerializer>();

        public static IServiceCollection AddSaml2MetadataSerializer<TSerializer>(this IServiceCollection services)
            where TSerializer : Saml2MetadataSerializer
        {
            services.TryAddTransient<Saml2MetadataSerializer, TSerializer>();
            return services;
        }
    }
}
