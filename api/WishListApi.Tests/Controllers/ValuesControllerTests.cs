using Amazon.Lambda.TestUtilities;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Controllers;

public class ValuesControllerTests
{
    [Fact]
    public async Task TestGet()
    {
        var request = TestControllerHelper.BuildGetRequest("/api/values");
        var context = new TestLambdaContext();

        var lambdaFunction = new LambdaEntryPoint();
        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        Assert.Equal(200, response.StatusCode);
        Assert.Equal("[\"value1\",\"value2\"]", response.Body);

        Assert.True(response.MultiValueHeaders.ContainsKey("Content-Type"));
        Assert.Equal("application/json; charset=utf-8", response.MultiValueHeaders["Content-Type"][0]);
    }

}
