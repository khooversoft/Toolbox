using Khooversoft.EventFlow;
using System.Linq;

namespace Khooversoft.Net
{
    public static class EventDetailContractExtensions
    {
        public static EventDetailContractV1 ConvertTo(this EventData self)
        {
            return new EventDetailContractV1
            {
                Timestamp = self.Timestamp,
                Date = self.Date,
                EventName = self.EventName,
                EventLevel = self.Level.ToString(),
                Tag = self.Tag,
                Message = self.Message,
                Properties = self.Properties.ToDictionary(i => i.Key, i => i.Value.ToString()),
            };
        }
    }
}
