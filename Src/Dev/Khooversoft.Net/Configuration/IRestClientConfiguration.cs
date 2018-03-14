using System.Collections.Generic;
using System.Net.Http;

namespace Khooversoft.Net
{
    public interface IRestClientConfiguration
    {
        HttpClient HttpClient { get; }

        IReadOnlyDictionary<string, object> Properties { get; }

        IRestClientConfiguration WithProperty<T>(T value) where T : class;
    }
}
