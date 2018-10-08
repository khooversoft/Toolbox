// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Calculate HMAC signature.
    /// Order is: method, URL.AbsolutePath, URL.Query (optional), headers (key.ToLower())
    /// 
    /// Signature = "credential:signature"
    /// 
    /// HMAC configuration has headers that can be used in signature.
    /// </summary>
    public class HmacSignature
    {
        private static Tag _tag = new Tag(nameof(HmacSignature));
        private readonly IHmacConfiguration _hmacConfiguration;
        private readonly TimeSpan _validFor;

        public HmacSignature(IHmacConfiguration hmacConfiguration, TimeSpan? validFor = null)
        {
            Verify.IsNotNull(nameof(hmacConfiguration), hmacConfiguration);

            _hmacConfiguration = hmacConfiguration;
            _validFor = validFor ?? TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// Create HMAC signature
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="url">URL</param>
        /// <param name="headers">headers in request</param>
        /// <returns>HMAC request</returns>
        public string CreateSignature(IWorkContext context, string credential, string hmacKey, string method, Uri url, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            Verify.IsNotEmpty(nameof(method), method);
            Verify.IsNotNull(nameof(url), url);
            Verify.IsNotNull(nameof(headers), headers);
            context = context.WithTag(_tag);

            return CreateSignatureInternal(
                context,
                credential: credential,
                hmacKey: hmacKey,
                hmacHeaders: _hmacConfiguration.Headers,
                method: method,
                url: url,
                headers: headers);
        }

        /// <summary>
        /// Validate HMAC signature
        /// </summary>
        /// <param name="signature">HMAC signature to verify</param>
        /// <param name="method">HTTP method</param>
        /// <param name="url">URL</param>
        /// <param name="headers">headers in request</param>
        /// <param name="date">date of request (optional, must be less then 5 minutes</param>
        /// <returns></returns>
        public bool ValidateSignature(IWorkContext context, string signature, string hmacKey, string method, Uri url, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, DateTimeOffset? date = null)
        {
            Verify.IsNotEmpty(nameof(signature), signature);
            context = context.WithTag(_tag);

            string[] signatureParts = signature.Split(':');
            if (signatureParts.Length != 2)
            {
                return false;
            }

            string calculatedSignature = CreateSignatureInternal(
                context,
                credential: signatureParts[0],
                hmacKey: hmacKey,
                hmacHeaders: _hmacConfiguration.Headers,
                method: method,
                url: url,
                headers: headers);

            if (calculatedSignature != signature)
            {
                return false;
            }

            if (date != null)
            {
                return (DateTimeOffset.UtcNow - (DateTimeOffset)date) > _validFor;
            }

            return true;
        }

        /// <summary>
        /// Internal create signature
        /// </summary>
        /// <param name="credential">credential</param>
        /// <param name="hmacKey">HMAC key</param>
        /// <param name="hmacHeaders">Valid HMAC headers</param>
        /// <param name="method">HTTP request method</param>
        /// <param name="url">HTTP request URL</param>
        /// <param name="headers">HTTP request headers</param>
        /// <returns></returns>
        private static string CreateSignatureInternal(IWorkContext context, string credential, string hmacKey, IEnumerable<string> hmacHeaders, string method, Uri url, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            Verify.IsNotEmpty(nameof(credential), credential);
            Verify.IsNotEmpty(nameof(hmacKey), hmacKey);
            Verify.IsNotNull(nameof(hmacHeaders), hmacHeaders);
            const string lineFeed = "\n";
            context = context.WithTag(_tag);

            using (var sha256 = SHA256.Create())
            {
                var list = new List<string>()
                {
                    credential,
                    method,
                    Uri.EscapeUriString(url.AbsolutePath.ToLowerInvariant()),
                    Uri.EscapeUriString(url.Query.ToLowerInvariant()),
                };

                IList<KeyValuePair<string, IEnumerable<string>>> signedHeadersList = headers
                    .Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key.Trim().ToLowerInvariant(), x.Value.Select(y => y.Trim())))
                    .Where(x => hmacHeaders.Contains(x.Key, StringComparer.OrdinalIgnoreCase))
                    .OrderBy(x => x.Key)
                    .ToList();

                if (signedHeadersList.Count > 0)
                {
                    IEnumerable<string> canonicalHeaders = signedHeadersList
                        .Select(x => x.Key + ':' + x.Value.Aggregate(","));

                    list.AddRange(canonicalHeaders);
                }

                string canonicalRequest = list
                    .Where(x => x.IsNotEmpty())
                    .Aggregate(lineFeed) + lineFeed;

                context.EventLog.Verbose(context, $"hmacKey={hmacKey}, canonicalRequest={canonicalRequest}");

                byte[] canonicalRequestContent = Encoding.UTF8.GetBytes(canonicalRequest);
                byte[] canonicalRequestContentHash = sha256.ComputeHash(canonicalRequestContent);
                string stringToSign = Convert.ToBase64String(canonicalRequestContentHash);

                byte[] secretKeyBytes = Encoding.UTF8.GetBytes(hmacKey);
                byte[] stringToSignBytes = Encoding.UTF8.GetBytes(stringToSign);

                using (var hmac = new HMACSHA256(secretKeyBytes))
                {
                    var signatureBytes = hmac.ComputeHash(stringToSignBytes);
                    var requestSignature = Convert.ToBase64String(signatureBytes);

                    return $"{credential}:{requestSignature}";
                }
            }
        }
    }
}
