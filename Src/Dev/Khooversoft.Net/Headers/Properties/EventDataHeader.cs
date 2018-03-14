using Khooversoft.Toolbox;

namespace Khooversoft.Net
{
    public class EventDataHeader : IHttpHeaderProperty
    {
        public EventDataHeader(string value)
        {
            Verify.IsNotEmpty(nameof(value), value);

            Value = value;
        }

        public EventDataHeader(string[] values)
        {
            Verify.IsNotNull(nameof(values), values);
            Verify.Assert(values.Length > 0, nameof(values));

            Value = values[0];
        }

        public static string HeaderKey { get; } = "API-EventData";

        public string Key { get; } = EventDataHeader.HeaderKey;

        public string Value { get; }

        public string FormatValueForHttp()
        {
            return Value;
        }
    }
}
