using System;
using System.Collections.Specialized;
using System.Web;
#pragma warning disable 1591

namespace PMAuth.Extensions
{
    public static class HttpExtensions
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            if (name == null || value == null)
            {
                return uri;
            }
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var uriBuilder = new UriBuilder(uri)
            {
                Query = httpValueCollection.ToString() ?? string.Empty
            };

            return uriBuilder.Uri;
        }
    }
}