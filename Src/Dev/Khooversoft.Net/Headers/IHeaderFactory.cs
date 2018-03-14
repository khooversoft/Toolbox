using System;

namespace Khooversoft.Net
{
    public interface IHeaderFactory
    {
        void Add(string key, Func<string[], IHttpHeaderProperty> factory);

        IHttpHeaderProperty Create(string key, string[] values);
    };
}
