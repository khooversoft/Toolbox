using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.EventFlow
{
    /// <summary>
    /// Event data from ETW Event Source
    /// </summary>
    [DebuggerDisplay("Timestamp={Timestamp}, EventId={EventId}, EventName={EventName}, EventSourceName={EventSourceName}")]
    public class EventData
    {
        private readonly List<KeyValuePair<string, object>> _propertyList;
        private readonly Deferred<IReadOnlyDictionary<string, object>> _propertyListDictionary;

        private readonly Deferred<string> _eventMessage;
        private readonly Deferred<string> _tag;
        private readonly Deferred<CvData> _cv;
        private readonly Deferred<string> _propertyMessage;

        private EventData()
        {
            _propertyListDictionary = new Deferred<IReadOnlyDictionary<string, object>>(() => _propertyList.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase));
            _eventMessage = new Deferred<string>(BuildEventMessage);
            _tag = new Deferred<string>(() => _propertyListDictionary.Value.Get<string>("tag"));
            _cv = new Deferred<CvData>(() => _propertyListDictionary.Value.Get<string>("Cv").If(x => x != null, x => new CvData(x), x => null));
            _propertyMessage = new Deferred<string>(() => _propertyListDictionary.Value.Get<string>("message"));
        }

        /// <summary>
        /// Construct from EventSource event
        /// </summary>
        /// <param name="eventArgs">event args</param>
        public EventData(EventWrittenEventArgs eventArgs)
            : this()
        {
            Verify.IsNotNull(nameof(eventArgs), eventArgs);

            Timestamp = HighResolutionDateTime.GetTimestamp();
            Date = DateTimeOffset.FromFileTime(Timestamp);

            ActivityId = eventArgs.ActivityId;
            Channel = eventArgs.Channel;
            EventId = eventArgs.EventId;
            EventName = eventArgs.EventName;
            EventSourceName = eventArgs.EventSource.Name;
            Keywords = eventArgs.Keywords;
            Level = eventArgs.Level;
            Opcode = eventArgs.Opcode;
            RelatedActivityId = eventArgs.RelatedActivityId;
            Tags = eventArgs.Tags;
            Task = eventArgs.Task;
            Version = eventArgs.Version;
            MessageFormat = eventArgs.Message;

            _propertyList = new List<KeyValuePair<string, object>>();
            for (int i = 0; i < eventArgs.PayloadNames.Count; i++)
            {
                _propertyList.Add(new KeyValuePair<string, object>(eventArgs.PayloadNames[i], eventArgs.Payload[i]));
            }
        }

        public DateTimeOffset Date { get; }

        public long Timestamp { get; }

        public Guid ActivityId { get; }

        public EventChannel Channel { get; }

        public int EventId { get; }

        public string EventName { get; }

        public string EventSourceName { get; }

        public EventKeywords Keywords { get; }

        public EventLevel Level { get; }

        public EventOpcode Opcode { get; }

        /// <summary>
        /// List of properties
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _propertyList;

        public Guid RelatedActivityId { get; }

        public EventTags Tags { get; }

        public EventTask Task { get; }

        public int Version { get; }

        public string MessageFormat { get; }

        public string EventMessage => _eventMessage.Value;

        public string Cv => _cv.Value.Cv;

        public string BaseCv => _cv.Value.BasesCv;

        public string Tag => _tag.Value;

        public string Message => _propertyMessage.Value;

        public IReadOnlyDictionary<string, object> Property => _propertyListDictionary.Value;

        /// <summary>
        /// Build formatted message, deferred work
        /// </summary>
        /// <returns>formatted message</returns>
        private string BuildEventMessage()
        {
            if (MessageFormat.IsEmpty())
            {
                return string.Empty;
            }

            try
            {
                return string.Format(MessageFormat, Properties.Select(x => x.Value).ToArray());
            }
            catch (Exception ex)
            {
                return $"Exception in message format {MessageFormat}, Ex={ex.Message}";
            }
        }

        /// <summary>
        /// Cv data
        /// </summary>
        private class CvData
        {
            public CvData()
            {
            }

            public CvData(string cv)
            {
                Cv = cv;

                int index = cv.IndexOf('.');
                if (index >= 0)
                {
                    BasesCv = cv.Substring(0, index);
                }
                else
                {
                    BasesCv = cv;
                }
            }

            public string Cv { get; }

            public string BasesCv { get; }
        }
    }
}
