using System.IdentityModel.Tokens.Jwt;
using AutoFixture.Xunit2;
using Shouldly;
using WishListApi.Config;
using WishListApi.Services;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Services;

public class AuthServiceTests
{
    private readonly JwtConfig _jwtConfig;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _jwtConfig = JwtConfigHelper.Get();
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        _sut = new AuthService(_jwtConfig);
    }

    [Theory, AutoData]
    public void GetJwtToken_should_return_valid_token(string email, string name)
    {
        var tokenString = _sut.GetJwtToken(email, name);

        tokenString.ShouldNotBeNullOrWhiteSpace();

        var result = _jwtSecurityTokenHandler.ReadJwtToken(tokenString);

        result.ShouldNotBeNull();
        result.Header.ShouldContainKeyAndValue("alg", "HS256");
        result.Header.ShouldContainKeyAndValue("typ", "JWT");

        result.Payload.ShouldContainKeyAndValue("aud", _jwtConfig.Audience);
        result.Payload.ShouldContainKeyAndValue("iss", _jwtConfig.Issuer);
        result.Payload.ShouldContainKeyAndValue("sub", name.Replace("-", "").ToLower());
        result.Payload.ShouldContainKeyAndValue("email", email.ToLower());
        result.Payload.ShouldContainKeyAndValue("name", name);

        result.ValidTo.ShouldBeLessThanOrEqualTo(DateTime.UtcNow.AddMinutes(61));
    }
}
