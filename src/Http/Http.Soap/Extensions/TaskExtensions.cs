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
                source.Task.ContinueWith(t => callback(t), TaskScheduler.Default);
            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    source.TrySetCanceled();
                else if (t.IsFaulted)
                    source.TrySetException(t.Exception!.InnerExceptions);
                else
                    source.TrySetResult(null);
            }, TaskScheduler.Default);
            return source.Task;
        }
        public static IAsyncResult ConvertToAsyncResult<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            var source = new TaskCompletionSource<TResult>(state);
            if (callback != null)
                source.Task.ContinueWith(t => callback(t), TaskScheduler.Default);
            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    source.TrySetCanceled();
                else if (t.IsFaulted)
                    source.TrySetException(t.Exception!.InnerExceptions);
                else
                    source.TrySetResult(t.Result);
            }, TaskScheduler.Default);
            return source.Task;
        }
    }
}
