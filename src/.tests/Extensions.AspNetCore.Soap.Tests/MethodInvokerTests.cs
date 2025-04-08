using Microsoft.Extensions.Logging;
using Moq;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Solid.Extensions.AspNetCore.Soap.Tests
{
    public class MethodInvokerTests
    {
        private string _value;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public async Task ShouldInvokeSynchronousMethod(string expected)
        {
            var method = GetMethod(nameof(SynchronousEcho));
            var invoker = new MethodInvoker(Mock.Of<ILogger<MethodInvoker>>());
            var result = await invoker.InvokeMethodAsync(this, method, new[] { expected });

            Assert.NotNull(result);
            Assert.False(result.IsVoid);
            Assert.False(result.IsAsync);
            Assert.Equal(expected, result.Result as string);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public async Task ShouldInvokeSynchronousVoidMethod(string expected)
        {
            var method = GetMethod(nameof(SynchronousVoid));
            var invoker = new MethodInvoker(Mock.Of<ILogger<MethodInvoker>>());
            var result = await invoker.InvokeMethodAsync(this, method, new[] { expected });

            Assert.NotNull(result);
            Assert.True(result.IsVoid);
            Assert.False(result.IsAsync);
            Assert.Null(result.Result);
            Assert.Equal(expected, _value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public async Task ShouldInvokeSynchronousOutVoidMethod(string expected)
        {
            var method = GetMethod(nameof(SynchronousOutVoid));
            var invoker = new MethodInvoker(Mock.Of<ILogger<MethodInvoker>>());
            var arguments = new MethodParameter[] { new MethodParameter { Name = "value", Value = expected }, new MethodParameter { Name = "copy", Out = true } };
            var result = await invoker.InvokeMethodAsync(this, method, arguments);

            Assert.NotNull(result);
            Assert.True(result.IsVoid);
            Assert.False(result.IsAsync);
            Assert.Null(result.Result);
            Assert.Equal(expected, _value);
            Assert.Contains("copy", result.OutParameters);
            Assert.Equal(expected, result.OutParameters["copy"]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public async Task ShouldInvokeAsynchronousMethod(string expected)
        {
            var method = GetMethod(nameof(AsynchronousEcho));
            var invoker = new MethodInvoker(Mock.Of<ILogger<MethodInvoker>>());
            var result = await invoker.InvokeMethodAsync(this, method, new[] { expected });

            Assert.NotNull(result);
            Assert.False(result.IsVoid);
            Assert.True(result.IsAsync);
            Assert.Equal(expected, result.Result as string);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public async Task ShouldInvokeAsynchronousVoidMethod(string expected)
        {
            var method = GetMethod(nameof(AsynchronousVoid));
            var invoker = new MethodInvoker(Mock.Of<ILogger<MethodInvoker>>());
            var result = await invoker.InvokeMethodAsync(this, method, new[] { expected });

            Assert.NotNull(result);
            Assert.True(result.IsVoid);
            Assert.True(result.IsAsync);
            Assert.Null(result.Result);
            Assert.Equal(expected, _value);
        }

        private MethodInfo GetMethod(string name) => GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

        private void SynchronousOutVoid(string value, out string copy) => _value = copy = value;
        private void SynchronousVoid(string value) => _value = value;

        private string SynchronousOutEcho(string value, out string copy) => copy = value;
        private string SynchronousEcho(string value) => value;

        private Task AsynchronousVoid(string value) 
        {
            _value = value;
            return Task.CompletedTask;
        }

        private Task<string> AsynchronousEcho(string value) => Task.FromResult(value);
    }
}
