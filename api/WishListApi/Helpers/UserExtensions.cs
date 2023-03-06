using System.Security.Claims;

namespace WishListApi.Helpers
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal? user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // The JWT "sub" claim is mapped to http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
            var userId = user.Claims.FirstOrDefault(c => c.Type.EndsWith("nameidentifier"))?.Value;

            // should never happen as Authentication middleware would check for "sub" claim on JWT token
            if (userId == null)
            {
                throw new ArgumentException("Could not retrieve user from Authorization header", nameof(user));
            }

            return userId;
        }
    }
}
