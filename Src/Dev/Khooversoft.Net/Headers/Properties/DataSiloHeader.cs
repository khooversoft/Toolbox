using Khooversoft.Toolbox;

namespace Khooversoft.Net
{
    public class DataSiloHeader : IHttpHeaderProperty
    {
        public DataSiloHeader(string value)
        {
            Verify.IsNotEmpty(nameof(value), value);
            Value = value;
        }

        public DataSiloHeader(string[] values)
        {
            Verify.IsNotNull(nameof(values), values);
            Verify.Assert(values.Length > 0, nameof(values));

            Value = values[0];
        }

        public static string HeaderKey { get; } = "API-Silo";

        public string Key { get; } = HeaderKey;

        public string Value { get; }

        public string FormatValueForHttp()
        {
            return Value;
        }
    }
}
