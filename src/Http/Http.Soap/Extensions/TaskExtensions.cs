using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks
{
    internal static class TaskExtensions
    {
        public static IAsyncResult ConvertToAsyncResult(this Task task, AsyncCallback callback, object state)
        {
            var source = new TaskCompletionSource<object>(state);
            if (callback != null)
                source.Task.ContinueWith(t => callback(t));
            task.ContinueWith(t =>
            {
                if (t.IsCompleted)
                    source.SetResult(null);
                else if (t.IsFaulted)
                    source.SetException(t.Exception);
            });
            return source.Task;
        }
        public static IAsyncResult ConvertToAsyncResult<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            var source = new TaskCompletionSource<TResult>(state);
            if (callback != null)
                source.Task.ContinueWith(t => callback(t));
            task.ContinueWith(t =>
            {
                if (t.IsCompleted)
                    source.SetResult(t.Result);
                else if (t.IsFaulted)
                    source.SetException(t.Exception);
            });
            return source.Task;
        }
    }
}
