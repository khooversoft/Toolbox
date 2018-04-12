// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Observers;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.EventFlow
{
    /// <summary>
    /// This observer will store the last (n) event data.  It uses a ring queue
    /// that will strictly enforce number of item restriction.
    /// </summary>
    public class EventDataBufferObserver : ObserverBase<EventData>, IEventDataBuffer
    {
        private readonly RingQueue<EventData> _eventRing;

        public EventDataBufferObserver(int ringSize = 1000)
        {
            _eventRing = new RingQueue<EventData>(ringSize);
        }

        protected override void OnNextCore(EventData value)
        {
            _eventRing.Enqueue(value);
        }

        public IEnumerable<EventData> SearchBuffer(Func<EventData, bool> test)
        {
            foreach (var item in _eventRing.ToList())
            {
                if (test(item))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<EventData> SearchForBaseCv(string baseCv, int lastRowCount = 100)
        {
            Verify.IsNotEmpty(nameof(baseCv), baseCv);
            Verify.Assert(lastRowCount > 0, $"{lastRowCount} must be greater than zero");

            int skipCount = Math.Max(_eventRing.Count - lastRowCount, 0);
            return SearchBuffer(x => x.Cv.SafeSubstring(baseCv.Length) == baseCv).Skip(skipCount);
        }
    }
}
