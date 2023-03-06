using WishListApi.Config;

namespace WishListApi.Tests.TestHelpers
{
    public static class JwtConfigHelper
    {
        public static JwtConfig Get()
        {
            // TODO: can we load this from appsettings.Development.json instead of hard coding?
            return new JwtConfig
            {
                Audience = "WishListFrontend",
                Issuer = "WishListApi",
                Secret = "d8f1b8a6-fd40-44b9-93b4-f8270ad61bc9"
            };
        }
    }
}
