/*
https://social.msdn.microsoft.com/Forums/en-US/163ef755-ff7b-4ea5-b226-bbe8ef5f4796/is-there-a-pattern-for-calling-an-async-method-synchronously?forum=async
https://gist.github.com/ChrisMcKee/6664438
*/


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.AsyncInline
{
    public static class AsyncInline
    {
        public static void Run(Func<Task> item)
        {
            Func<Task<object>> wrappedTask = async () =>
            {
                await item();
                return null;
            };
            Run(wrappedTask);
        }

        public static T Run<T>(Func<Task<T>> item)
        {
            T ret = default(T);

            var originalContext = SynchronizationContext.Current;
            var newContext = new ExclusiveSynchronizationContext();

            SynchronizationContext.SetSynchronizationContext(newContext);

            try
            {
                newContext.Post(
                    async _ =>
                    {
                        try
                        {
                            ret = await item();
                        }
                        finally
                        {
                            newContext.EndMessageLoop();
                        }
                    }, null);

                newContext.BeginMessageLoop();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(originalContext);
            }

            return ret;
        }


        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            readonly Queue<Tuple<SendOrPostCallback, object>> items =
            new Queue<Tuple<SendOrPostCallback, object>>();
            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
