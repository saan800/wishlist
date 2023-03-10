using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WishListApi.Config;
using WishListApi.Helpers;

namespace WishListApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtConfig _jwtConfig;

        public AuthService(JwtConfig jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }

        public string GetJwtToken(string email, string name)
        {
            email = email.ToLower().Trim();
            name = name.Trim();
            // TODO: something unique here that's not a GUID hopefully - use for SeoUrlKeys of wish lists
            var userId = WishListHelper.FormatKey(name);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                    issuer: _jwtConfig.Issuer,
                    audience: _jwtConfig.Audience,
                    // standard JWT claims: https://www.iana.org/assignments/jwt/jwt.xhtml
                    claims: new List<Claim>
                    {
                        new Claim("sub", userId),
                        new Claim("email", email),
                        new Claim("name", name)
                    },
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signinCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
