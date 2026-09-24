using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Solid.Http.Soap;

namespace Solid.Http;

public static class SolidHttpClientFactoryExtensions
{
    public static T CreateProxy<T>(this ISolidHttpClientFactory factory, Binding binding, Action<ProxyOptions<T>> configure)
    {
        var options = new ProxyOptions<T>();
        configure(options);
        return factory.CreateProxy<T>(binding, options);
    }

    public static T CreateProxy<T>(this ISolidHttpClientFactory factory, Binding binding, ProxyOptions<T> options)
    {
        var address = options.Address.Uri;
        var client = factory.CreateWithBaseAddress($"{address.Scheme}://{address.Authority}");
        return client.CreateProxy<T>(binding, options);
    }
    
    public static T CreateProxy<T>(this ISolidHttpClient client, Binding binding, Action<ProxyOptions<T>> configure)
    {
        var options = new ProxyOptions<T>();
        configure(options);
        return client.CreateProxy<T>(binding, options);
    }
    
    public static T CreateProxy<T>(this ISolidHttpClient client, Binding binding, ProxyOptions<T> options)
    {
        var custom = binding.WithSolidHttpTransport(client);
        ConfigureBinding(custom, options);
        var factory = CreateChannelFactory(custom, options);
        return factory.CreateChannel();
    }

    private static void ConfigureBinding<T>(CustomBinding binding, ProxyOptions<T> options)
    {
        if (options.ConfigureBinding != null)
            options.ConfigureBinding(binding);
        
        binding.Elements.Find<MessageEncodingBindingElement>().MessageVersion = options.MessageVersion;
        
        
    }

    private static T CreateChannel<T>(ChannelFactory<T> factory, ProxyOptions<T> options)
    {
        if (options.CreateChannel != null)
            return options.CreateChannel(factory);

        return factory.CreateChannel();
    }

    private static ChannelFactory<T> CreateChannelFactory<T>(Binding binding, ProxyOptions<T> options)
    {
        if (options.CreateChannelFactory != null)
            return options.CreateChannelFactory(binding, options.Address);
        
        return new ChannelFactory<T>(binding, options.Address);
    }
}