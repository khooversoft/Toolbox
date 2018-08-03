using FluentAssertions;
using Khooversoft.Toolbox.Security;
using System;
using System.Collections.Generic;
using Xunit;

namespace Khooversoft.Toolbox.Test.Security
{
    [Trait("Category", "HMAC")]
    public class HmacTests
    {
        private static Tag _tag = new Tag(nameof(HmacTests));
        private static IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public void SignatureHeaderTest()
        {
            var context = _workContext.WithTag(_tag);

            var testHeaders = new List<KeyValuePair<string, IEnumerable<string>>>
            {
                new KeyValuePair<string, IEnumerable<string>>("Content-Type", new string[] { "application/x-www-form-urlencoded", "charset=utf-8" }),
                new KeyValuePair<string, IEnumerable<string>>("Content-MD5", new string[] { "kdskflosifm3938dldasksdfjdf" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-Cv", new string[] { "dkdfjdie.1" }),
            };

            var uri = new Uri("http://localhost:4096/resource1/field");
            const string method = "POST";
            const string apiKey = "c4afb1cc5771d871763a393e44b703571b55cc28424d1a5e86da6ed3c154a4b9";
            const string credential = "credential@domain.com";

            IHmacConfiguration hmacConfiguration = new HmacConfiguration();
            var hmac = new HmacSignature(hmacConfiguration);
            string signature = hmac.CreateSignature(context, credential, apiKey, method, uri, testHeaders);

            hmac.ValidateSignature(context, signature, apiKey, method, uri, testHeaders).Should().BeTrue();
        }

        [Fact]
        public void SignatureExtraHeaderTest()
        {
            var context = _workContext.WithTag(_tag);

            var testHeaders = new List<KeyValuePair<string, IEnumerable<string>>>
            {
                new KeyValuePair<string, IEnumerable<string>>("Content-Type", new string[] { "application/x-www-form-urlencoded", "charset=utf-8" }),
                new KeyValuePair<string, IEnumerable<string>>("Content-MD5", new string[] { "kdskflosifm3938dldasksdfjdf" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-Cv", new string[] { "dkdfjdie.1" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-Key", new string[] { "3asfvesef" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-duplicate", new string[] { "false" }),
            };

            var uri = new Uri("http://localhost:4096/resource1/field");
            const string method = "POST";
            const string apiKey = "c4afb1cc5771d871763a393e44b703571b55cc28424d1a5e86da6ed3c154a4b9";
            const string credential = "credential@domain.com";

            IHmacConfiguration hmacConfiguration = new HmacConfiguration();
            var hmac = new HmacSignature(hmacConfiguration);
            string signature = hmac.CreateSignature(context, credential, apiKey, method, uri, testHeaders);

            hmac.ValidateSignature(context, signature, apiKey, method, uri, testHeaders).Should().BeTrue();
        }

        [Fact]
        public void SignatureFailureExtraHeaderTest()
        {
            var context = _workContext.WithTag(_tag);

            var testHeaders = new List<KeyValuePair<string, IEnumerable<string>>>
            {
                new KeyValuePair<string, IEnumerable<string>>("Content-Type", new string[] { "application/x-www-form-urlencoded", "charset=utf-8" }),
                new KeyValuePair<string, IEnumerable<string>>("Content-MD5", new string[] { "kdskflosifm3938dldasksdfjdf" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-Cv", new string[] { "dkdfjdie.1" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-Key", new string[] { "3asfvesef" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-duplicate", new string[] { "false" }),
            };

            var uri = new Uri("http://localhost:4096/resource1/field");
            const string method = "POST";
            const string apiKey = "c4afb1cc5771d871763a393e44b703571b55cc28424d1a5e86da6ed3c154a4b9";
            const string credential = "credential@domain.com";

            IHmacConfiguration hmacConfiguration = new HmacConfiguration();
            var hmac = new HmacSignature(hmacConfiguration);
            string signature = hmac.CreateSignature(context, credential, apiKey, method, uri, testHeaders);

            signature = signature.Remove(2, 2);
            hmac.ValidateSignature(context, signature, apiKey, method, uri, testHeaders).Should().BeFalse();
        }

        [Fact]
        public void SignatureFailureTest()
        {
            var context = _workContext.WithTag(_tag);

            var testHeaders = new List<KeyValuePair<string, IEnumerable<string>>>
            {
                new KeyValuePair<string, IEnumerable<string>>("Content-Type", new string[] { "application/x-www-form-urlencoded", "charset=utf-8" }),
                new KeyValuePair<string, IEnumerable<string>>("Content-MD5", new string[] { "kdskflosifm3938dldasksdfjdf" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-Cv", new string[] { "dkdfjdie.1" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-Key", new string[] { "3asfvesef" }),
                new KeyValuePair<string, IEnumerable<string>>("Api-duplicate", new string[] { "false" }),
            };

            var uri = new Uri("http://localhost:4096/resource1/field");
            const string method = "POST";
            const string apiKey = "c4afb1cc5771d871763a393e44b703571b55cc28424d1a5e86da6ed3c154a4b9";
            const string credential = "credential@domain.com";

            IHmacConfiguration hmacConfiguration = new HmacConfiguration();
            var hmac = new HmacSignature(hmacConfiguration);
            string signature = hmac.CreateSignature(context, credential, apiKey, method, uri, testHeaders);

            signature = signature.Remove(credential.Length + 5, 2);
            hmac.ValidateSignature(context, signature, apiKey, method, uri, testHeaders).Should().BeFalse();
        }
    }
}
