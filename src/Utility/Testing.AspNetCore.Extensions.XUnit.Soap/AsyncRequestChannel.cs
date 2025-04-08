using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    abstract class AsyncRequestChannel : IRequestChannel
    {
        protected virtual TimeSpan DefaultTimeout => TimeSpan.FromSeconds(5);

        public abstract EndpointAddress RemoteAddress { get; }

        public virtual Uri Via => null;

        public CommunicationState State { get; private set; }

        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;

        protected virtual ValueTask AbortAsync() => new ValueTask();
        protected virtual ValueTask OpenAsync(TimeSpan timeout) => new ValueTask();
        protected virtual ValueTask CloseAsync(TimeSpan timeout) => new ValueTask();
        protected abstract Task<Message> RequestAsync(Message request, TimeSpan timeout);

        public void Abort() => AbortAsync().GetAwaiter().GetResult();

        public IAsyncResult BeginClose(AsyncCallback callback, object state) => BeginClose(DefaultTimeout, callback, state);

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            => InternalCloseAsync(timeout).ConvertToAsyncResult(callback, state);

        public IAsyncResult BeginOpen(AsyncCallback callback, object state) => BeginOpen(DefaultTimeout, callback, state);

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            => InternalOpenAsync(timeout).ConvertToAsyncResult(callback, state);

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state) => BeginRequest(message, DefaultTimeout, callback, state);

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            => RequestAsync(message, timeout).ConvertToAsyncResult(callback, state);

        public void Close() => Close(DefaultTimeout);

        public void Close(TimeSpan timeout) => InternalCloseAsync(timeout).GetAwaiter().GetResult();

        public void EndClose(IAsyncResult result) => End(result);

        public void EndOpen(IAsyncResult result) => End(result);

        public Message EndRequest(IAsyncResult result) => GetResult<Message>(result);

        public T GetProperty<T>() where T : class => default;

        public void Open() => Open(DefaultTimeout);

        public void Open(TimeSpan timeout) => InternalOpenAsync(timeout).GetAwaiter().GetResult();

        public Message Request(Message message) => Request(message, DefaultTimeout);

        public Message Request(Message message, TimeSpan timeout) => RequestAsync(message, timeout).GetAwaiter().GetResult();
        
        async Task InternalOpenAsync(TimeSpan timeout)
        {
            State = CommunicationState.Opening;
            if (Opening != null) Opening.Invoke(this, EventArgs.Empty);
            await OpenAsync(timeout);
            State = CommunicationState.Opened;
            if (Opened != null) Opened.Invoke(this, EventArgs.Empty);
        }
        async Task InternalCloseAsync(TimeSpan timeout)
        {
            State = CommunicationState.Closing;
            if (Closing != null) Closing.Invoke(this, EventArgs.Empty);
            await CloseAsync(timeout);
            State = CommunicationState.Closed;
            if (Closed != null) Closed.Invoke(this, EventArgs.Empty);
        }

        T GetResult<T>(IAsyncResult result)
        {
            if (!(result is Task<T> task)) return default;

            End(task);

            return task.Result;
        }

        void End(IAsyncResult result)
        {
            if (!(result is Task task)) return;

            if (task.IsFaulted)
            {
                State = CommunicationState.Faulted;
                if (Faulted != null)
                    Faulted.Invoke(this, EventArgs.Empty);
                throw task.Exception;
            }
            if (task.IsCanceled) throw new TaskCanceledException();
        }
    }
}
