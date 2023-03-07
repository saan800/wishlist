using System.Net;
using System.Text.Json;
using AutoFixture.Xunit2;
using Shouldly;
using WishListApi.Models.Auth;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Controllers;

public class AuthControllerTests : WishListApiTests.BaseControllerTests
{
    public AuthControllerTests(WishListApiTests wishListApiTests) : base(wishListApiTests)
    {
    }

    [Fact]
    public async Task FakeLogin_should_error_when_no_body()
    {
        // when
        var (responseStatusCode, errorResult) = await Invoke<IDictionary<string, IReadOnlyCollection<string>>>
                                                    (HttpMethod.Post, "/auth/fakelogin", "");

        // then
        responseStatusCode.ShouldBe(HttpStatusCode.BadRequest);

        errorResult.ShouldNotBeNull();
        errorResult.ShouldContainKey("login");
        errorResult["login"].ShouldNotBeEmpty();
    }

    [Fact]
    public async Task FakeLogin_should_error_when_body_is_not_json()
    {
        // when
        var (responseStatusCode, errorResult) = await Invoke<IDictionary<string, IReadOnlyCollection<string>>>
                                                    (HttpMethod.Post, "/auth/fakelogin", "email: this@email.com");

        // then
        responseStatusCode.ShouldBe(HttpStatusCode.BadRequest);

        errorResult.ShouldNotBeNull();
        errorResult.ShouldContainKey("login");
        errorResult["login"].ShouldNotBeEmpty();
    }

    [Theory]
    [InlineAutoData(null)]
    [InlineAutoData("")]
    [InlineAutoData("no-at.sign")]
    public async Task FakeLogin_should_error_if_login_email_is_invalid(string email)
    {
        // when
        var body = JsonSerializer.Serialize(new FakeLoginRequest { Email = email, Name = "Bugs Bunny" });
        var (responseStatusCode, errorResult) = await Invoke<IDictionary<string, IReadOnlyCollection<string>>>
                                                    (HttpMethod.Post, "/auth/fakelogin", body);

        // then
        responseStatusCode.ShouldBe(HttpStatusCode.BadRequest);

        errorResult.ShouldNotBeNull();
        errorResult.ShouldContainKey("Email");
        errorResult["Email"].ShouldNotBeEmpty();
        errorResult.ShouldNotContainKey("Name");
    }

    [Theory]
    [InlineAutoData(null)]
    [InlineAutoData("")]
    public async Task FakeLogin_should_error_if_login_name_invalid(string name)
    {
        // when
        var body = JsonSerializer.Serialize(new FakeLoginRequest { Email = "bugs.bunny@email.com", Name = name });
        var (responseStatusCode, errorResult) = await Invoke<IDictionary<string, IReadOnlyCollection<string>>>
                                                    (HttpMethod.Post, "/auth/fakelogin", body);

        // then
        responseStatusCode.ShouldBe(HttpStatusCode.BadRequest);

        errorResult.ShouldNotBeNull();
        errorResult.ShouldNotContainKey("Email");
        errorResult.ShouldContainKey("Name");
        errorResult["Name"].ShouldNotBeEmpty();
    }

    [Theory, AutoData]
    public async Task FakeLogin_should_return_Jwt_token(string email, string name)
    {
        // when
        email = $"{email}@Email.com ";
        var body = JsonSerializer.Serialize(new FakeLoginRequest { Email = email, Name = name });
        var (responseStatusCode, responseBodyObj) = await Invoke<JwtTokenResponse>
                                                    (HttpMethod.Post, "/auth/fakelogin", body);

        // then
        responseStatusCode.ShouldBe(HttpStatusCode.OK);

        responseBodyObj.ShouldNotBeNull();
        responseBodyObj.Email.ShouldBe(email.ToLower().Trim());
        responseBodyObj.Name.ShouldBe(name);
        responseBodyObj.Token.ShouldNotBeNullOrWhiteSpace();
    }
}
