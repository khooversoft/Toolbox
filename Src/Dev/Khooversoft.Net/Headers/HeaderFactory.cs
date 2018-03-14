using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;

namespace Khooversoft.Net
{
    public class HeaderFactory : IHeaderFactory
    {
        public Dictionary<string, Func<string[], IHttpHeaderProperty>> _headers = new Dictionary<string, Func<string[], IHttpHeaderProperty>>(StringComparer.OrdinalIgnoreCase)
        {
            [CvHeader.HeaderKey] = x => new CvHeader(x),
            [DataSiloHeader.HeaderKey] = x => new DataSiloHeader(x),
            [TestDumpHeader.HeaderKey] = x => new TestDumpHeader(x),
        };

        public void Add(string key, Func<string[], IHttpHeaderProperty> factory)
        {
            Verify.IsNotEmpty(nameof(key), key);
            Verify.IsNotNull(nameof(factory), factory);

            _headers.Add(key, factory);
        }

        public IHttpHeaderProperty Create(string key, string[] values)
        {
            Verify.IsNotEmpty(nameof(key), key);

            Func<string[], IHttpHeaderProperty> createClass;
            if (!_headers.TryGetValue(key, out createClass))
            {
                return null;
            }

            return createClass(values);
        }
    }
}
