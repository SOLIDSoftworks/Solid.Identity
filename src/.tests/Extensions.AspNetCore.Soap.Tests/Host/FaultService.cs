using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Tests.Host
{
    class FaultService : IFaultServiceContract, IDetailedFaultServiceContract
    {
        public void ThrowsContractFault(string detailMessage) => throw new FaultException<TestDetail>(new TestDetail { Message = detailMessage });

        public void ThrowsException(string message) => throw new Exception(message);
    }
}
