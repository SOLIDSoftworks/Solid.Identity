using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Channels;

namespace Solid.Http.Soap;

public class ProxyOptions<T>
{
    public MessageVersion MessageVersion { get; set; } = MessageVersion.Default;
    public EndpointAddress Address { get; set; }
    
    public Func<Binding, EndpointAddress, ChannelFactory<T>> CreateChannelFactory { get; set; }
    public Func<ChannelFactory<T>, T> CreateChannel { get; set; }
    public Action<Binding> ConfigureBinding { get; set; }
}