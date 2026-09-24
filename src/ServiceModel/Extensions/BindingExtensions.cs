using Solid.Extensions.ServiceModel;

namespace System.ServiceModel.Channels;

public static class BindingExtensions
{
    public static void Apply(this Binding binding, ProxyOptions options)
    {
        binding.SendTimeout = options.SendTimeout;
        binding.ReceiveTimeout = options.ReceiveTimeout;
        binding.OpenTimeout = options.OpenTimeout;
        binding.CloseTimeout = options.CloseTimeout;
        
        if(binding is HttpBindingBase http)
            http.Apply(options);
        if(binding is WSHttpBindingBase wsHttp)
            wsHttp.Apply(options);
    }

    private static void Apply(this HttpBindingBase binding, ProxyOptions options)
    {
        binding.MaxBufferPoolSize = options.MaxBufferPoolSize;
        binding.MaxReceivedMessageSize = options.MaxReceivedMessageSize;
        
        binding.ReaderQuotas.MaxArrayLength = options.ReaderQuotasMaxArrayLength;
        binding.ReaderQuotas.MaxStringContentLength = options.ReaderQuotasMaxStringContentLength;
        binding.ReaderQuotas.MaxDepth = options.ReaderQuotasMaxDepth;
    }
    private static void Apply(this WSHttpBindingBase binding, ProxyOptions options)
    {
        binding.MaxBufferPoolSize = options.MaxBufferPoolSize;
        binding.MaxReceivedMessageSize = options.MaxReceivedMessageSize;
        
        binding.ReaderQuotas.MaxArrayLength = options.ReaderQuotasMaxArrayLength;
        binding.ReaderQuotas.MaxStringContentLength = options.ReaderQuotasMaxStringContentLength;
        binding.ReaderQuotas.MaxDepth = options.ReaderQuotasMaxDepth;
    }
}