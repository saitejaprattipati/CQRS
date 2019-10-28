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
    }
}
