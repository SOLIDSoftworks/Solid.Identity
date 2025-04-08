using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Tests.Host
{
    public class EchoService : IEchoServiceContract
    {
        public Task<string> AsynchronousEchoAsync(string value) => Task.FromResult(value);

        public string Echo(string value) => value;

        public void OutEcho(string value, out string echo) => echo = value;

        public EchoWrapper WrappedAndOutEcho(EchoWrapper value, out string copy)
        {
            copy = value.Value;
            return value;
        }

        public EchoWrapper WrappedEcho(EchoWrapper value) => value;
    }
}
