using System.Net;
using System.Text.Json;
using Amazon.Lambda.TestUtilities;
using Moq;
using Shouldly;
using WishListApi.Models.WishList;
using WishListApi.Services;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Controllers;

public class WishListsControllerTests : WishListApiTests.BaseControllerTests
{
    public WishListsControllerTests(WishListApiTests wishListApiTests) : base(wishListApiTests)
    {
    }

    [Theory]
    [AutoDomainData]
    public async Task Get(IList<WishList> wishLists, Mock<IWishListStore> mockWistListStore)
    {
        // when
        var (responseStatusCode, responseObj) = await Invoke<List<WishList>>
                                                    (HttpMethod.Get, "/api/wishlists", "");

        // then
        responseStatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // TODO: replace below with cleaner above
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
