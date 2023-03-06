using System.Text.Json;
using Amazon.Lambda.TestUtilities;
using Moq;
using Shouldly;
using WishListApi.Models.WishList;
using WishListApi.Services;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Controllers;

public class WishListsControllerTests
{
    // TODO: can't figure out how to Mock IWishListStore into TestLambdaEntryPoint yet ;(
    //      might need to change to integration test with DynamoDb running in docker
    //      Or maybe do add then get, update, get, delete, get process to check
    [Theory]
    [AutoDomainData]
    public async Task Get(IList<WishList> wishLists, Mock<IWishListStore> mockWistListStore)
    {
        var request = TestControllerHelper
                        .BuildGetRequest("/api/wishlists")
                        .AddJwtAuthorizationHeader("some@email.com", "someone");

        mockWistListStore
            .Setup(x => x.GetForUser(It.IsAny<string>()))
            .ReturnsAsync(wishLists);

        var lambdaFunction = new TestLambdaEntryPoint(new List<ServiceOverride>{
            new ServiceOverride(typeof(IWishListStore), mockWistListStore.Object)
        });

        var response = await lambdaFunction.FunctionHandlerAsync(request, new TestLambdaContext());

        response.StatusCode.ShouldBe(200);

        response.Body.ShouldNotBeNullOrWhiteSpace();
        var result = JsonSerializer.Deserialize<List<WishList>>(response.Body);
        result.ShouldBeEquivalentTo(wishLists);
    }
}
