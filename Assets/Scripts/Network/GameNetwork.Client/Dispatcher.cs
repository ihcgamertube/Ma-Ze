using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameNetwork.Client
{
    public abstract class Dispatcher
    {
        public static Dispatcher FromCurrentThread()
        {
            return new SynchronizationContextDispatcher(SynchronizationContext.Current);
        }

        public abstract void Invoke(Action action);
        public abstract void BeginInvoke(Action action);
    }

    internal class QueuedDispatcher : Dispatcher
    {
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private readonly AutoResetEvent _syncInvoke = new AutoResetEvent(true);

        public override void Invoke(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _syncInvoke.Reset();
            _queue.Enqueue(() =>
            {
                action();
                _syncInvoke.Set();
            });
            _syncInvoke.WaitOne();
        }

        public override void BeginInvoke(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _queue.Enqueue(action);
        }

        public void InvokeAll()
        {
            while (!_queue.TryDequeue(out var action))
            {
                action();
            }
        }
    }

    internal class SynchronizationContextDispatcher : Dispatcher
    {
        private readonly SynchronizationContext _context;

        public SynchronizationContextDispatcher(SynchronizationContext context)
        {
            _context = context;
        }

        public override void Invoke(Action action)
        {
            _context.Send(state => action(), null);
        }

        public override void BeginInvoke(Action action)
        {
            _context.Post(state => action(), null);
        }

    }
}
