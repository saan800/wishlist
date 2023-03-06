using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WishListApi.Attrubutes;
using WishListApi.Config;
using WishListApi.Models;

namespace WishListApi.Controllers;

[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtConfig _jwtConfig;

    public AuthController(JwtConfig jwtConfig)
    {
        this._jwtConfig = jwtConfig;
    }

    // POST auth/fakelogin
    [HttpPost("fakelogin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public IActionResult FakeLogin([FromBody] FakeLoginRequest login)
    {
        // Email and Name will always have values as the ModelState is already checked
        var email = login.Email?.ToLower().Trim() ?? "";
        var name = login.Name?.Trim() ?? "";

        return Ok(BuildJwtTokenResponse(email, name));
    }

#pragma warning disable S1135 // Track uses of "TODO" tags
    // TODO: SSO login for Google, Facebook etc - return same JWT token format
    //       Check if email address is known in DB, create new user record if needed 
#pragma warning restore S1135 // Track uses of "TODO" tags

    private JwtTokenResponse BuildJwtTokenResponse(string email, string name)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                // standard JWT claims: https://www.iana.org/assignments/jwt/jwt.xhtml
                claims: new List<Claim>{
                    new Claim("sub", email),
                    new Claim("name", name)
                },
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signinCredentials
            );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return new JwtTokenResponse
        {
            Email = email,
            Name = name,
            Token = tokenString
        };
    }
}
