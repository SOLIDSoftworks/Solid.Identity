using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Tests.Host
{
    [ServiceContract]
    public interface IEchoServiceContract
    {
        [OperationContract]
        string Echo(string value);
        [OperationContract]
        EchoWrapper WrappedEcho(EchoWrapper value);
        [OperationContract]
        EchoWrapper WrappedAndOutEcho(EchoWrapper value, out string copy);
        [OperationContract(AsyncPattern = true)]
        Task<string> AsynchronousEchoAsync(string value);
        [OperationContract]
        void OutEcho(string value, out string echo);
    }

    [DataContract]
    public class EchoWrapper
    {
        [DataMember]
        public string Value { get; set; }
    }
}
