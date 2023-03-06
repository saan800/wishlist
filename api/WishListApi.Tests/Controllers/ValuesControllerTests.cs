using Amazon.Lambda.TestUtilities;
using Shouldly;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Controllers;

public class ValuesControllerTests
{
    [Fact]
    public async Task TestGet()
    {
        var request = TestControllerHelper
                        .BuildGetRequest("/api/values")
                        .AddJwtAuthorizationHeader("some@email.com", "someone");
        var context = new TestLambdaContext();

        var lambdaFunction = new TestLambdaEntryPoint();

        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        response.StatusCode.ShouldBe(200);
        response.Body.ShouldBe("[\"value1\",\"value2\"]");

        response.MultiValueHeaders.ShouldContainKey("Content-Type");
        response.MultiValueHeaders["Content-Type"][0].ShouldContain("application/json");
        response.MultiValueHeaders["Content-Type"][0].ShouldContain("charset=utf-8");
    }

}
