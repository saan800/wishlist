using System.Text.RegularExpressions;

namespace WishListApi.Helpers
{
    public static class WishListHelper
    {
        public static string FormatSeoUrlKey(string userId, string title)
        {

            return $"{FormatKey(userId)}/{FormatKey(title)}";
        }

        public static string FormatKey(string val)
        {
            var rgx = new Regex("[^a-zA-Z0-9-. ]");

            val = rgx.Replace(val, "");
            return val
                    .Trim()
                    .ToLower()
                    .Replace("  ", " ")
                    .Replace(" ", "-");
        }
    }
}
