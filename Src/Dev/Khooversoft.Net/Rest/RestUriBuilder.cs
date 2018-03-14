using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Khooversoft.Net
{
    /// <summary>
    /// REST URI builder using UriBuilder (builder pattern using fluent pattern)
    /// </summary>
    public class RestUriBuilder
    {
        public enum Schemes
        {
            File,
            Ftp,
            Gopher,
            Http,
            Https,
            Mailto,
            News
        };

        private KeyValuePair<string, string>? _credentials;
        private readonly Dictionary<string, string> _queryParams = new Dictionary<string, string>();

        public RestUriBuilder()
        {
        }

        public RestUriBuilder(string baseUri)
        {
            Verify.IsNotEmpty(nameof(baseUri), baseUri);

            BaseUri = new Uri(baseUri);
        }

        public RestUriBuilder(Uri baseUri)
        {
            Verify.IsNotNull(nameof(baseUri), baseUri);

            BaseUri = baseUri;
        }

        public Uri BaseUri { get; private set; }
        public string Fragment { get; private set; }
        public string Host { get; private set; }
        public StringVector Path { get; private set; }
        public Schemes? Scheme { get; private set; }
        public bool RemovePort { get; private set; }
        public int? Port { get; private set; }
        public IEnumerable<KeyValuePair<string, string>> QueryParameters { get { return _queryParams; } }

        public RestUriBuilder Clear()
        {
            _credentials = null;
            _queryParams.Clear();
            BaseUri = null;
            Fragment = null;
            Host = null;
            Scheme = null;
            RemovePort = false;
            Port = null;

            return this;
        }

        public RestUriBuilder SetBaseUri(string baseUri)
        {
            BaseUri = new Uri(baseUri);
            return this;
        }

        public RestUriBuilder SetBaseUri(Uri baseUri)
        {
            BaseUri = baseUri;
            return this;
        }

        public RestUriBuilder SetFragement(string value)
        {
            Fragment = value;
            return this;
        }

        public RestUriBuilder SetHost(string value)
        {
            Host = value;
            return this;
        }

        public RestUriBuilder SetRemovePort(bool value = true)
        {
            RemovePort = value;
            return this;
        }

        public RestUriBuilder SetCredentials(string user, string password)
        {
            Verify.IsNotEmpty(nameof(user), user);

            _credentials = new KeyValuePair<string, string>(user, password);
            return this;
        }

        public RestUriBuilder ClearCredentials()
        {
            _credentials = null;
            return this;
        }

        public RestUriBuilder SetPath(string value)
        {
            if (value.IsEmpty())
            {
                Path = null;
                return this;
            }

            Path = new StringVector(value);
            return this;
        }

        public RestUriBuilder AddPath(string value)
        {
            Verify.IsNotEmpty(nameof(value), value);

            Path = this?.Path.With(value) ?? StringVector.Parse(value);
            return this;
        }

        public RestUriBuilder AddPath(IEnumerable<string> values)
        {
            Verify.IsNotNull(nameof(values), values);

            Path = this?.Path?.With(values) ?? new StringVector(values);
            return this;
        }

        public RestUriBuilder AddPath(StringVector vector)
        {
            Verify.IsNotNull(nameof(vector), vector);

            Path = this?.Path?.With(vector) ?? vector;
            return this;
        }

        public RestUriBuilder SetPort(int? port)
        {
            Verify.Assert(port == null || (int)port >= -1 && (int)port <= 65535, "port should be between -1 and 65535, inclusive");

            Port = port;
            return this;
        }

        public RestUriBuilder SetDefaultPort()
        {
            Port = null;
            return this;
        }

        public RestUriBuilder SetScheme(Schemes scheme)
        {
            Scheme = scheme;
            return this;
        }

        public RestUriBuilder AddQuery<T>(string key, T value)
        {
            Verify.IsNotEmpty(nameof(key), key);
            Verify.IsNotNull(nameof(value), value);

            var valueStr = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            Verify.IsNotEmpty(nameof(value), valueStr);
            _queryParams.Add(key, valueStr);

            return this;
        }

        public RestUriBuilder AddQuery<T>(IEnumerable<KeyValuePair<string, T>> parameters)
        {
            Verify.IsNotNull(nameof(parameters), parameters);

            foreach (var item in parameters)
            {
                AddQuery(item.Key, item.Value);
            }

            return this;
        }

        public RestUriBuilder ClearParameters()
        {
            _queryParams.Clear();
            return this;
        }

        public Uri Build()
        {
            var uriBuilder = BaseUri == null
                ? new UriBuilder()
                : new UriBuilder(BaseUri);

            if (Fragment.IsNotEmpty())
            {
                uriBuilder.Fragment = Fragment;
            }

            if (Host.IsNotEmpty())
            {
                uriBuilder.Host = Host;
            }

            if (Path.IsNotNull())
            {
                uriBuilder.Path = Path;
            }

            if (Scheme.IsNotNull())
            {
                uriBuilder.Scheme = ((Schemes)Scheme).ToString();
            }

            if (Port != null)
            {
                uriBuilder.Port = (int)Port;
            }
            else if (RemovePort)
            {
                uriBuilder.Port = -1;
            }

            if (_credentials != null)
            {
                uriBuilder.UserName = _credentials.Value.Key;
                uriBuilder.Password = _credentials.Value.Value;
            }

            if (_queryParams.Count > 0)
            {
                uriBuilder.Query = string.Join("&", _queryParams.Select(x => Uri.EscapeDataString(x.Key).Trim() + "=" + Uri.EscapeDataString(x.Value).Trim()));
            }

            return uriBuilder.Uri;
        }

        public override string ToString()
        {
            return Build().AbsoluteUri;
        }
    }
}
