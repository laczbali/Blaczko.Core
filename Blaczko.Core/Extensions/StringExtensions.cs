using System.Web;

namespace Blaczko.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if the string is null or all whitespaces, returns false otherwise
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value is null) return true;
            value = value.Trim();
            return value.Length == 0;
        }

        /// <summary>
        /// Returns null if the string is null or empty, otherwise returns the input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NullIfEmpty(this string value, bool trim = true)
        {
            if (value.IsNullOrWhiteSpace()) return null;
            return trim ? value.Trim() : value;
        }

        /// <summary>
        /// Strips whitespace, normalizes url ending
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string TidyUrl(this string url)
        {
            return new Uri(url).AbsoluteUri;
        }

        /// <summary>
        /// Trims and URL encodes each relativePart, returns the full absolute Uri
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativePart"></param>
        /// <returns></returns>
        public static string TidyUrl(this string baseUrl, params string[] relativePart)
        {
            relativePart = relativePart
                .Select(x => x.NullIfEmpty())
                .Where(x => x is not null)
                .Select(x => HttpUtility.UrlEncode(x))
                .ToArray();

            var relativePath = string.Join("/", relativePart);

            return new Uri(new Uri(baseUrl), relativePath).AbsoluteUri;
        }
    }
}
