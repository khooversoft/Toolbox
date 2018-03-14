using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    public static class PageResultExtensions
    {
        /// <summary>
        /// Generate continuation URL based on page result which uses Index.
        /// </summary>
        /// <typeparam name="T">type of page result</typeparam>
        /// <param name="self">page result</param>
        /// <param name="baseUri">base URL for service</param>
        /// <param name="controllerPath">controller part of the URL</param>
        /// <param name="path">path (optional)</param>
        /// <returns>Continuation URL</returns>
        public static Uri BuildContinueUri<T>(this PageResult<T> self, Uri baseUri, string controllerPath, string path = null)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(baseUri), baseUri);
            Verify.IsNotEmpty(nameof(controllerPath), controllerPath);

            if (self.Index.IsEmpty())
            {
                return null;
            }

            var build = new UriBuilder(baseUri);
            string currentPath = build.Path;

            build.Path = null;
            build.Query = null;
            baseUri = build.Uri;

            var paths = controllerPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Concat(path != null ? path.Split('/') : Enumerable.Empty<string>())
                .ToList();

            return new RestUriBuilder(baseUri)
                .AddPath(paths)
                .AddQuery("limit", self.Limit)
                .AddQuery("index", self.Index)
                .Build();
        }
    }
}
