// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Reminder scheduler will support multiple timed events where a delegate will be called with a message.  For example, placing a 10 second interval
    /// will cause the delegate to receive the signal in 10 seconds.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageScheduler<T> : IObservable<T>, IDisposable
    {
        private static readonly Tag _tag = new Tag(nameof(MessageScheduler<T>));
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private Subject<T> _subject = new Subject<T>();
        private Timer _timer;
        private int _actionLock;
        private readonly object _lock = new object();
        private Stack<InternalMessage> _messageStack = new Stack<InternalMessage>();
        private Func<Stack<InternalMessage>> _getStack;

        public MessageScheduler()
        {
            _timer = new Timer(TimerInvokeAction);
        }

        /// <summary>
        /// Is signal running
        /// </summary>
        public bool IsRunning { get; private set; } = true;

        /// <summary>
        /// Send message now
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        public MessageScheduler<T> Now(T message)
        {
            lock (_lock)
            {
                AddMessage(DateTime.Now, message);
            }

            StartTimer();
            return this;
        }

        /// <summary>
        /// Add interval and run if currently started
        /// </summary>
        /// <param name="interval">interval to run</param>
        /// <param name="message">message to send</param>
        /// <returns>this</returns>
        public MessageScheduler<T> Schedule(TimeSpan interval, T message)
        {
            lock (_lock)
            {
                DateTime eventDate = DateTime.Now + interval;
                AddMessage(eventDate, message);
            }

            StartTimer();
            return this;
        }

        /// <summary>
        /// Add intervals and run if currently started
        /// </summary>
        /// <param name="messages">enumeration of intervals and message (key = Interval, value = message)</param>
        /// <returns>this</returns>
        public MessageScheduler<T> Schedule(IEnumerable<KeyValuePair<TimeSpan, T>> messages)
        {
            Verify.IsNotNull(nameof(messages), messages);

            lock (_lock)
            {
                DateTime now = DateTime.Now;

                foreach (var item in messages)
                {
                    DateTime eventDate = now + item.Key;
                    AddMessage(eventDate, item.Value);
                }
            }

            StartTimer();
            return this;
        }

        /// <summary>
        /// Clear all intervals
        /// </summary>
        /// <returns>this</returns>
        public MessageScheduler<T> Clear()
        {
            lock (_lock)
            {
                _messageStack.Clear();
            }

            return this;
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <returns>this</returns>
        public MessageScheduler<T> Start()
        {
            lock (_lock)
            {
                _timer = _timer ?? new Timer(TimerInvokeAction);
                IsRunning = true;
            }

            StartTimer();
            return this;
        }

        /// <summary>
        /// Stop signal schedules
        /// </summary>
        /// <returns></returns>
        public MessageScheduler<T> Stop()
        {
            lock (_lock)
            {
                IsRunning = false;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            return this;
        }

        /// <summary>
        /// Dispose of the timer and shut down
        /// </summary>
        public void Dispose()
        {
            IsRunning = false;

            lock (_lock)
            {
                var timer = Interlocked.Exchange(ref _timer, null);
                timer?.Dispose();
            }
        }

        /// <summary>
        /// Start the timer if running and there are intervals
        /// The timer is set based on the lowest interval and removed from the list
        /// </summary>
        private void StartTimer()
        {
            lock (_lock)
            {
                if (!IsRunning || _getStack().Count == 0)
                {
                    _timer?.Change(Timeout.Infinite, Timeout.Infinite);
                    return;
                }

                InternalMessage internalMessage = _getStack().Peek();
                TimeSpan ts = internalMessage.EventDate - DateTime.Now;
                _timer.Change(Math.Max((int)ts.TotalMilliseconds, 0), Timeout.Infinite);
            }
        }

        /// <summary>
        /// Invoke action (single thread, no lock)
        /// </summary>
        /// <param name="state">message</param>
        private void TimerInvokeAction(object state)
        {
            var context = _workContext.WithTag(_tag);

            if (!IsRunning)
            {
                return;
            }

            int currentValue = Interlocked.CompareExchange(ref _actionLock, 1, 0);
            if (currentValue == 1)
            {
                return;
            }

            try
            {
                ObserversEventSource.Log.Verbose(context, nameof(TimerInvokeAction));

                // Get messages first to minimize the time in lock
                List<T> messages = new List<T>();
                lock (_lock)
                {
                    while (_getStack().Count > 0)
                    {
                        if (_messageStack.Peek().IsReady)
                        {
                            messages.Add(_messageStack.Pop().Message);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Send messages, must be outside the lock because the called delegate code
                // can send additional messages.
                foreach (var item in messages)
                {
                    _subject.OnNext(item);
                }

                StartTimer();
            }
            finally
            {
                int value = Interlocked.CompareExchange(ref _actionLock, 0, 1);
                Verify.Assert<InvalidOperationException>(value == 1, "Nonblocking lock reset error");
            }
        }

        /// <summary>
        /// Push new message onto the stack.  Will set delegate to re-sort the stack
        /// </summary>
        /// <param name="eventDate">event date</param>
        /// <param name="message">message</param>
        private void AddMessage(DateTime eventDate, T message)
        {
            eventDate -= TimeSpan.FromMilliseconds(10);
            _messageStack.Push(new InternalMessage(eventDate, message));
            _getStack = SortStack;
        }

        /// <summary>
        /// Sort the message stack based on date
        /// </summary>
        /// <returns></returns>
        private Stack<InternalMessage> SortStack()
        {
            _getStack = () => _messageStack;

            if (_messageStack.Count < 2)
            {
                return _messageStack;
            }

            _messageStack = new Stack<InternalMessage>(_messageStack.OrderByDescending(x => x.EventDate));
            return _messageStack;
        }

        /// <summary>
        /// Allows subscriptions to message
        /// </summary>
        /// <param name="observer">observer</param>
        /// <returns>disposable used to unsubscribe</returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        /// <summary>
        /// Internal message to track interval and n-messages
        /// </summary>
        [DebuggerDisplay("Interval={Interval}, Messages.Count={Messages.Count}")]
        private class InternalMessage
        {
            public InternalMessage(DateTime eventDate, T message)
            {
                EventDate = eventDate;
                Message = message;
            }

            public DateTime EventDate { get; }

            public T Message { get; }

            /// <summary>
            /// Can send message
            /// </summary>
            public bool IsReady
            {
                get { return EventDate <= DateTime.Now; }
            }
        }
    }
}
