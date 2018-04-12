// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Worker queue is a class that is an observer (receiving messages) as well as an observable
    /// (sending messages).  The worker queue allows the message to be transferred to another thread
    /// for processing.  Messages received are placed in a ring buffer to be processed by a task.
    /// 
    /// The task will process all the messages in the ring buffer and send them to any
    /// observers that have subscribed (i.e. observable).
    /// </summary>
    /// <typeparam name="T">queue data type</typeparam>
    public class WorkerQueue<T> : IObservable<T>, IObserver<T>
    {
        private readonly static Tag _tag = new Tag(nameof(WorkerQueue<T>));
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private readonly Subject<T> _subject = new Subject<T>();
        private readonly RingQueue<T> _queue;
        private readonly AutoResetEvent _notifyNewMessage = new AutoResetEvent(false);
        private readonly object _drainLock = new object();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _queueTask;

        public WorkerQueue(int maxQueueSize)
        {
            Verify.Assert(maxQueueSize > 0, $"{maxQueueSize} must be greater than 0");

            _queue = new RingQueue<T>(maxQueueSize);
            _queueTask = Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Is running
        /// </summary>
        public bool Running { get { return !_cancellationTokenSource.IsCancellationRequested; } }

        /// <summary>
        /// Return the count in the ring queue
        /// </summary>
        public int Count { get { return _queue.Count; } }

        /// <summary>
        /// Return the lost count from the ring queue
        /// </summary>
        public int LostCount { get { return _queue.LostCount; } }

        /// <summary>
        /// Enqueue item
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(T value)
        {
            Verify.Assert<InvalidOperationException>(Running, "must be running");

            _queue.Enqueue(value);
            _notifyNewMessage.Set();
        }

        public void OnError(Exception error)
        {
            Verify.Assert<InvalidOperationException>(Running, "must be running");
            ObserversEventSource.Log.Error(_workContext.WithTag(_tag), $"Queue count={_queue.Count}", error);
            Shutdown();
            _subject.OnError(error);
        }

        public void OnCompleted()
        {
            Verify.Assert<InvalidOperationException>(Running, "must be running");
            ObserversEventSource.Log.Verbose(_workContext.WithTag(_tag), $"Queue count={_queue.Count}");

            Shutdown();

            lock (_drainLock)
            {
                InternalProcessQueue(true);
            }

            _subject.OnCompleted();
        }

        /// <summary>
        /// Subscribe to messages from the ring buffer
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        /// <summary>
        /// Thread that is processing the queue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>task</returns>\
        private void ProcessQueue()
        {
            if (Running)
            {
                InternalProcessQueue(false);
            }
        }

        /// <summary>
        /// Internal method to processes the internal queue
        /// </summary>
        /// <param name="drain"></param>
        private void InternalProcessQueue(bool drain)
        {
            lock (_drainLock)
            {
                while (!_cancellationTokenSource.IsCancellationRequested || drain)
                {
                    if (_queue.IsEmpty && !drain)
                    {
                        _notifyNewMessage.WaitOne(500);
                    }

                    while (!_cancellationTokenSource.IsCancellationRequested || drain)
                    {
                        (bool Success, T data) rtn = _queue.TryDequeue();
                        if (!rtn.Success)
                        {
                            if (drain)
                            {
                                return;
                            }

                            break;
                        }

                        _subject.OnNext(rtn.data);
                    }
                }
            }
        }

        /// <summary>
        /// Shutdown timer
        /// </summary>
        private void Shutdown()
        {
            _cancellationTokenSource.Cancel();
            _notifyNewMessage.Set();

            Task t = Interlocked.Exchange(ref _queueTask, null);
            t?.Wait();

            ObserversEventSource.Log.Verbose(_workContext.WithTag(_tag), "Exit");
        }
    }
}
