using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using WishListApi.Services;

namespace WishListApi.Tests.TestHelpers
{
    public static class ApiGatewayProxyRequestExtensions
    {
        #region Headers

        private static IAuthService _authService;
        private static IAuthService GetAuthService()
        {
            _authService ??= new AuthService(JwtConfigHelper.Get());
            return _authService;
        }

        public static APIGatewayProxyRequest AddJwtAuthorizationHeader(this APIGatewayProxyRequest request, string email, string name)
        {
            var tokenString = GetAuthService().GetJwtToken(email, name);
            return request.AddHeader("Authorization", $"Bearer {tokenString}");
        }

        public static APIGatewayProxyRequest AddHeader(this APIGatewayProxyRequest request, string key, string value)
        {
            request.Headers ??= new Dictionary<string, string>();
            request.Headers.Add(key, value);
            return request;
        }

        #endregion

        #region Body

        public static APIGatewayProxyRequest AddObjToBodyAsJson(this APIGatewayProxyRequest request, Object obj)
        {
            if (obj != null)
            {
                request.Body = JsonSerializer.Serialize(obj);
            }
            return request;
        }

        public static APIGatewayProxyRequest AddBody(this APIGatewayProxyRequest request, string val)
        {
            if (!string.IsNullOrWhiteSpace(val))
            {
                request.Body = val;
            }
            return request;
        }

        #endregion
    }
}
