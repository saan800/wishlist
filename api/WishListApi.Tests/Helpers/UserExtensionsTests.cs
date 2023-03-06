using System.Security.Claims;
using Shouldly;
using WishListApi.Helpers;
using Xunit;

namespace WishListApi.Tests.Helpers;

public class UserExtensionsTests
{
    [Fact]
    public void GetUserId_when_null_should_throw_exeption()
    {
        ClaimsPrincipal? user = null;

        Should.Throw<ArgumentNullException>(() =>
        {
            user.GetUserId();
        });
    }

    [Fact]
    public void GetUserId_no_nameidentifier_claim_should_throw_exeption()
    {

        var identity = new ClaimsIdentity
        (
            new List<Claim>
            {
                new Claim("email", "fred.flintstone@email.com"),
                new Claim("name", "Fred Flintstone")
            },
            "JWT"
        );
        var user = new ClaimsPrincipal(identity);

        Should.Throw<ArgumentException>(() =>
        {
            user.GetUserId();
        });
    }

    [Fact]
    public void GetUserId_return_userId_when_have_nameidentifier_claim()
    {
        var userId = "fred123";
        var identity = new ClaimsIdentity
        (
            new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId),
                new Claim("email", "fred.flintstone@email.com"),
                new Claim("name", "Fred Flintstone")
            },
            "JWT"
        );
        var user = new ClaimsPrincipal(identity);

        var result = user.GetUserId();

        result.ShouldBe(userId);
    }
}
