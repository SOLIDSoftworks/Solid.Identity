using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Tests.Host
{
    [ServiceContract]
    public interface IFaultServiceContract
    {
        [OperationContract]
        void ThrowsException(string message);
        [OperationContract]
        [FaultContract(typeof(TestDetail))]
        void ThrowsContractFault(string detailMessage);
    }

    [ServiceContract]
    public interface IDetailedFaultServiceContract
    {
        [OperationContract]
        void ThrowsException(string message);
        [OperationContract]
        [FaultContract(typeof(TestDetail))]
        void ThrowsContractFault(string detailMessage);
    }

    [DataContract]
    public class TestDetail
    { 
        [DataMember]
        public string Message { get; set; }
    }
}
