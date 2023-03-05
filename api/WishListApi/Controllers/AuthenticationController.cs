using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WishListApi.Attrubutes;
using WishListApi.Config;
using WishListApi.Models;

namespace WishListApi.Controllers;

[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly JwtConfig _jwtConfig;

    public AuthenticationController(JwtConfig jwtConfig)
    {
        this._jwtConfig = jwtConfig;
    }

    // POST authentication/fakelogin
    [HttpPost("fakelogin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public IActionResult FakeLogin([FromBody] FakeLoginRequest login)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: new List<Claim>{
                    new Claim("email", login.Email ?? ""),
                    new Claim("given_name", login.FirstName ?? ""),
                    new Claim("family_name", login.LastName ?? ""),
                    new Claim("sub", _jwtConfig.Subject),
                },
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signinCredentials
            );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return Ok(new JwtTokenResponse
        {
            Token = tokenString
        });
    }
}
