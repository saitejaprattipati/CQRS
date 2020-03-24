using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Author.Core.Framework
{
    public static class Helper
    {
        public static string ReplaceChars(string originalString)
        {
            string replacedString = originalString;
            if (replacedString != null)
            {
                replacedString = Regex.Replace(replacedString.Replace("&", "-").Replace(" ", "-").Replace(".", "-"), @"[^\w\.@-]", "");
                replacedString = Regex.Replace(replacedString, @"[-]+", "-");
            }
            return replacedString;
        }

        public static bool ContainsAny(this string input, IEnumerable<string> containsKeywords, StringComparison comparisonType)
        {
            return containsKeywords.Any(keyword => input.IndexOf(keyword, comparisonType) >= 0);
        }
    }
}
