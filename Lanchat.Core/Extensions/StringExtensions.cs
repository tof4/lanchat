using System;

namespace Lanchat.Core.Extensions
{
    internal static class StringExtensions
    {
        internal static string Truncate(this string str, int maxLength)
        {
            str = str.Trim();
            if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(str);
            return str.Length <= maxLength ? str : str.Substring(0, maxLength) + "...";
        }
    }
}