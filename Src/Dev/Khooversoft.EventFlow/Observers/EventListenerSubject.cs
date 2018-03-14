using Khooversoft.Observers;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;

namespace Khooversoft.EventFlow
{
    /// <summary>
    /// Subscripts to ETW events and sends data to observables
    /// </summary>
    public class EventListenerSubject : EventListener, IObservable<EventData>, IDisposable
    {
        private WorkerQueue<EventData> _workerQueue = new WorkerQueue<EventData>(10000);

        public EventListenerSubject()
        {
        }

        /// <summary>
        /// Enable events from an EventSource
        /// </summary>
        /// <param name="eventSource">event source</param>
        /// <param name="eventLevel">event level</param>
        /// <returns></returns>
        public new EventListenerSubject EnableEvents(EventSource eventSource, EventLevel eventLevel)
        {
            Verify.IsNotNull(nameof(eventSource), eventSource);

            base.EnableEvents(eventSource, eventLevel);
            return this;
        }

        /// <summary>
        /// Enable events from an EventSource
        /// </summary>
        /// <param name="eventSource">event source</param>
        /// <param name="eventLevel">event level</param>
        /// <param name="matchAnyKeyword">matching key words</param>
        /// <returns></returns>
        public new EventListenerSubject EnableEvents(EventSource eventSource, EventLevel eventLevel, EventKeywords matchAnyKeyword)
        {
            Verify.IsNotNull(nameof(eventSource), eventSource);

            base.EnableEvents(eventSource, eventLevel, matchAnyKeyword);
            return this;
        }

        /// <summary>
        /// Enable events
        /// </summary>
        /// <param name="eventSource">event source</param>
        /// <param name="eventLevel">event level</param>
        /// <param name="matchAnyKeyword">match key words</param>
        /// <param name="arguments">arguments to be matched to enable the events</param>
        /// <returns></returns>
        public new EventListenerSubject EnableEvents(EventSource eventSource, EventLevel eventLevel, EventKeywords matchAnyKeyword, IDictionary<string, string> arguments)
        {
            Verify.IsNotNull(nameof(eventSource), eventSource);

            base.EnableEvents(eventSource, eventLevel, matchAnyKeyword, arguments);
            return this;
        }

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <param name="observer">observer to add</param>
        /// <returns>disposable object to clear subscriptions</returns>
        public IDisposable Subscribe(IObserver<EventData> observer)
        {
            return _workerQueue.Subscribe(observer);
        }

        /// <summary>
        /// Event receiver, called when event is received
        /// </summary>
        /// <param name="eventData">event data</param>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (_workerQueue?.Running == true)
            {
                _workerQueue.OnNext(new EventData(eventData));
            }
        }

        /// <summary>
        /// Complete and dispose of resources
        /// </summary>
        public new void Dispose()
        {
            WorkerQueue<EventData> worker = Interlocked.Exchange(ref _workerQueue, null);
            if (worker != null)
            {
                worker.OnCompleted();
                base.Dispose();
            }
        }
    }
}
