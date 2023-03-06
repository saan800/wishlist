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
        // TODO: get user 
        return Ok(BuildJwtTokenResponse(login.Email ?? "", login.Name ?? ""));
    }

    // TODO: SSO login for Google, Facebook etc - return same JWT token format
    //       Check if email is known in DB, create new user record if needed
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
