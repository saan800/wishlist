using System.Net;
using Microsoft.AspNetCore.Mvc;
using WishListApi.Attrubutes;
using WishListApi.Models.Auth;
using WishListApi.Services;

namespace WishListApi.Controllers;

[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Temporary login method until I get around to doing SSO
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    // POST auth/fakelogin
    [HttpPost("fakelogin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(typeof(JwtTokenResponse), (int)HttpStatusCode.OK)]
    public IActionResult FakeLogin([FromBody] FakeLoginRequest login)
    {
        // Email and Name will always have values as the ModelState is already checked in ValidationFilterAttribute
        return Ok(BuildJwtTokenResponse(login.Email ?? "", login.Name ?? ""));
    }

#pragma warning disable S1135 // Track uses of "TODO" tags
    // TODO: SSO login for Google, Facebook etc - return same JWT token format
    //       Check if email is known in DB, create new user record if needed
#pragma warning restore S1135 // Track uses of "TODO" tags

    private JwtTokenResponse BuildJwtTokenResponse(string email, string name)
    {
        email = email.ToLower().Trim();
        name = name.Trim();

        return new JwtTokenResponse
        {
            Email = email,
            Name = name,
            Token = _authService.GetJwtToken(email, name)
        };
    }
}
