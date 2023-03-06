using System.Text.RegularExpressions;

namespace WishListApi.Helpers
{
    public static class WishListHelper
    {
        public static string FormatSeoUrlKey(string userId, string title)
        {

            return $"{FormatForUrl(userId)}/{FormatForUrl(title)}";
        }

        private static string FormatForUrl(string val)
        {
            var rgx = new Regex("[^a-z0-9-.]");
            val = val
                    .Trim()
                    .ToLower()
                    .Replace("  ", " ")
                    .Replace(" ", "-");
            return rgx.Replace(val, "");
        }
    }
}
