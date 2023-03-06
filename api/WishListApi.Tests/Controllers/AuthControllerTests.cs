using System.Net;
using System.Text.Json;
using Amazon.Lambda.TestUtilities;
using AutoFixture.Xunit2;
using Shouldly;
using WishListApi.Models;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task FakeLogin_should_error_when_no_body()
    {
        var request = TestControllerHelper
                            .BuildPostRequest("/auth/fakelogin");
        var context = new TestLambdaContext();

        var lambdaFunction = new TestLambdaEntryPoint();

        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        response.Body.ShouldNotBeNullOrWhiteSpace();

        var errorResult = JsonSerializer.Deserialize<IDictionary<string, IReadOnlyCollection<string>>>(response.Body);

        errorResult.ShouldNotBeNull();
        errorResult.ShouldContainKey("login");
        errorResult["login"].ShouldNotBeEmpty();
    }

    [Fact]
    public async Task FakeLogin_should_error_when_body_is_not_json()
    {
        var request = TestControllerHelper
                            .BuildPostRequest("/auth/fakelogin")
                            .AddBody("email: this@email.com");
        var context = new TestLambdaContext();

        var lambdaFunction = new TestLambdaEntryPoint();

        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        response.Body.ShouldNotBeNullOrWhiteSpace();

        var errorResult = JsonSerializer.Deserialize<IDictionary<string, IReadOnlyCollection<string>>>(response.Body);

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
        var request = TestControllerHelper
                            .BuildPostRequest("/auth/fakelogin")
                            .AddObjToBodyAsJson(new FakeLoginRequest { Email = email, Name = "Fred Flintstone" });
        var context = new TestLambdaContext();

        var lambdaFunction = new TestLambdaEntryPoint();

        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        response.Body.ShouldNotBeNullOrWhiteSpace();

        var errorResult = JsonSerializer.Deserialize<IDictionary<string, IReadOnlyCollection<string>>>(response.Body);

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
        var request = TestControllerHelper
                            .BuildPostRequest("/auth/fakelogin")
                            .AddObjToBodyAsJson(new FakeLoginRequest { Email = "fred.flintstone@email.com", Name = name });
        var context = new TestLambdaContext();

        var lambdaFunction = new TestLambdaEntryPoint();

        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        response.Body.ShouldNotBeNullOrWhiteSpace();

        var errorResult = JsonSerializer.Deserialize<IDictionary<string, IReadOnlyCollection<string>>>(response.Body);
        errorResult.ShouldNotBeNull();
        errorResult.ShouldNotContainKey("Email");
        errorResult.ShouldContainKey("Name");
        errorResult["Name"].ShouldNotBeEmpty();
    }

    [Theory, AutoData]
    public async Task FakeLogin_should_return_Jwt_token(string email, string name)
    {
        email = $"{email}@Email.com ";
        var request = TestControllerHelper
                    .BuildPostRequest("/auth/fakelogin")
                    .AddObjToBodyAsJson(new FakeLoginRequest { Email = email, Name = name });
        var context = new TestLambdaContext();

        var lambdaFunction = new TestLambdaEntryPoint();

        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        response.Body.ShouldNotBeNullOrWhiteSpace();

        response.MultiValueHeaders.ShouldContainKey("Content-Type");
        response.MultiValueHeaders["Content-Type"][0].ShouldContain("application/json");
        response.MultiValueHeaders["Content-Type"][0].ShouldContain("charset=utf-8");

        var bodyResult = JsonSerializer.Deserialize<IDictionary<string, string>>(response.Body);
        bodyResult.ShouldNotBeNull();
        bodyResult.ShouldContainKeyAndValue("email", email.ToLower().Trim());
        bodyResult.ShouldContainKeyAndValue("name", name);
        bodyResult.ShouldContainKey("token");
        bodyResult["token"].ShouldNotBeNullOrWhiteSpace();
    }
}
