using AutoFixture.Xunit2;
using Moq;
using Shouldly;
using WishListApi.Helpers;
using WishListApi.Models.WishList;
using WishListApi.Services;
using WishListApi.Tests.TestHelpers;
using Xunit;

namespace WishListApi.Tests.Services;

public class WishListServiceTests
{
    #region GetAllForUser

    [Theory, AutoDomainData]
    public async Task GetAllForUser_store_has_no_wishlists_should_return_empty_list(
        string userId, [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        SetupStore(store);

        var result = await sut.GetAllForUser(userId);

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Theory, AutoDomainData]
    public async Task GetAllForUser_should_return_owned_wishlists_from_store(
        string userId, IList<WishList> wishlists,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        wishlists = wishlists.Select(wl =>
        {
            wl.UserId = userId;
            return wl;
        }).ToList();
        SetupStore(store, wishlists);

        var result = await sut.GetAllForUser(userId);

        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(wishlists.Count);
        result.ShouldBeEquivalentTo(wishlists.ToList());
    }

    [Theory, AutoDomainData]
    public async Task GetAllForUser_should_return_shared_wishlists_from_store(
        string userId, IList<WishList> wishlists,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        var sharedWishLists = wishlists.Select(wl => new SharedWishList { SeoUrlKey = wl.SeoUrlKey, UserId = userId });
        SetupStore(store, wishlists, sharedWishLists);

        var result = await sut.GetAllForUser(userId);

        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(wishlists.Count);
        result.ShouldBeEquivalentTo(wishlists.ToList());
    }

    [Theory, AutoDomainData]
    public async Task GetAllForUser_should_return_both_own_and_shared_wishlists_from_store(
        string userId, IList<WishList> wishlists, IList<WishList> otherWishlists,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        var allWishlists = wishlists
            .Select(wl =>
            {
                wl.UserId = userId;
                return wl;
            })
            .Concat(otherWishlists)
            .ToList();

        var sharedWishLists = otherWishlists.Select(wl => new SharedWishList { SeoUrlKey = wl.SeoUrlKey, UserId = userId });
        SetupStore(store, allWishlists, sharedWishLists);

        var result = await sut.GetAllForUser(userId);

        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(allWishlists.Count);
        result.ShouldBeEquivalentTo(allWishlists.ToList());
    }

    [Theory, AutoDomainData]
    public async Task GetAllForUser_should_return_wishlists_only_if_correct_userId(
        string userId, string otherUserId, IList<WishList> wishlists, IList<WishList> otherWishlists,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        var allWishlists = wishlists
            .Select(wl =>
            {
                wl.UserId = userId;
                return wl;
            })
            .Concat(otherWishlists)
            .ToList();

        var sharedWishLists = otherWishlists.Select(wl => new SharedWishList { SeoUrlKey = wl.SeoUrlKey, UserId = userId });
        SetupStore(store, allWishlists, sharedWishLists);

        var result = await sut.GetAllForUser(otherUserId);

        result.ShouldBeEmpty();
    }

    #endregion

    #region Get

    [Theory, AutoDomainData]
    public async Task Get_should_return_null_if_no_wish_list_for_user(
        string userId, string seoUrlKey, IList<WishList> wishlists,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        wishlists = wishlists.Select(wl =>
        {
            wl.UserId = userId;
            return wl;
        }).ToList();
        SetupStore(store, wishlists);

        var result = await sut.Get(seoUrlKey, userId);

        result.ShouldBeNull();
    }

    [Theory, AutoDomainData]
    public async Task Get_should_return_wish_list_when_exists_for_user(
        string userId, IList<WishList> wishlists,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        wishlists = wishlists.Select(wl =>
        {
            wl.UserId = userId;
            return wl;
        }).ToList();
        SetupStore(store, wishlists);
        var seoUrlKey = wishlists.Select(wl => wl.SeoUrlKey).First();

        var result = await sut.Get(seoUrlKey, userId);

        result.ShouldNotBeNull();
        result.ShouldBe(wishlists.First(wl => wl.SeoUrlKey.Equals(seoUrlKey)));
    }

    #endregion

    #region Create

    [Theory, AutoDomainData]
    public async Task Create_should_return_error_if_same_seoUrlKey_exists(
        string userId, string title,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        var wishList = new WishList
        {
            SeoUrlKey = WishListHelper.FormatSeoUrlKey(userId, title),
            Title = title,
            UserId = userId
        };
        SetupStore(store, new List<WishList> { wishList });
        var request = new CreateWishListRequest
        {
            Title = wishList.Title
        };

        (WishList? result, string? errorResult) = await sut.Create(request, userId);

        result.ShouldBeNull();
        errorResult?.ShouldContain($"{wishList.SeoUrlKey} already exists");
    }

    [Theory, AutoDomainData]
    public async Task Create_should_return_populated_WishList_object_when_created(
        string userId, string title,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        SetupStore(store);
        var request = new CreateWishListRequest
        {
            Title = title
        };

        (WishList? result, string? errorResult) = await sut.Create(request, userId);

        errorResult.ShouldBeNull();

        result.ShouldNotBeNull();
        result.SeoUrlKey.ShouldBe(WishListHelper.FormatSeoUrlKey(userId, title));
        result.Title.ShouldBe(title);
        result.UserId.ShouldBe(userId);
        result.CreatedDate.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow);
        result.LastUpdated.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow);

        result.Items.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
    }

    #endregion

    #region Update

    [Theory, AutoDomainData]
    public async Task Update_should_return_error_if_no_wish_list_exists_for_user(
        string userId, string seoUrlKey, string title,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        SetupStore(store);
        var request = new UpdateWishListRequest { Title = title };

        (WishList? result, string? errorResult) = await sut.Update(request, seoUrlKey, userId);

        result.ShouldBeNull();
        errorResult?.ShouldContain($"Could not find a wish list for {seoUrlKey}");
    }

    [Theory, AutoDomainData]
    public async Task Update_should_return_updated_WishList_object(
        string userId, string seoUrlKey, string title,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        var wishList = new WishList
        {
            SeoUrlKey = seoUrlKey,
            Title = "oldTitle",
            UserId = userId,
            Items = new List<WishListItem> { new WishListItem { Title = "Item I want" } },
            CreatedDate = DateTime.UtcNow.AddDays(-1),
            LastUpdated = DateTime.UtcNow.AddDays(-1)
        };
        SetupStore(store, new List<WishList> { wishList });
        var request = new UpdateWishListRequest { Title = title };

        (WishList? result, string? errorResult) = await sut.Update(request, seoUrlKey, userId);

        errorResult.ShouldBeNull();

        result.ShouldNotBeNull();
        result.SeoUrlKey.ShouldBe(wishList.SeoUrlKey);
        result.Title.ShouldBe(title);
        result.UserId.ShouldBe(wishList.UserId);
        result.CreatedDate.ShouldBe(wishList.CreatedDate);
        result.LastUpdated.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow);

        result.Items.ShouldBeEquivalentTo(wishList.Items);
    }

    #endregion

    #region Delete

    [Theory, AutoDomainData]
    public async Task Delete_should_request_deletion_from_store(
        string userId, string seoUrlKey,
        [Frozen] Mock<IWishListStore> store, WishListService sut
    )
    {
        SetupStore(store);

        await sut.Delete(seoUrlKey, userId);

        store.Verify(x => x.Delete(seoUrlKey, userId), Times.Once());
    }

    #endregion

    private void SetupStore(Mock<IWishListStore> store, IEnumerable<WishList>? wishLists = null, IEnumerable<SharedWishList>? sharedWishLists = null)
    {
        if (wishLists != null)
        {
            store
                .Setup(x => x.GetForUser(It.IsAny<string>()))
                .ReturnsAsync((string userId) => wishLists.Where(wl => wl.UserId == userId).ToList());
        }
        else
        {
            store
                .Setup(x => x.GetForUser(It.IsAny<string>()))
                .ReturnsAsync(null as IList<WishList>);
        }


        if (sharedWishLists != null && wishLists != null)
        {
            store
                .Setup(x => x.GetSharedWithUser(It.IsAny<string>()))
                .ReturnsAsync((string userId) =>
                {
                    var seoUrlKeys = sharedWishLists
                        .Where(wl => wl.UserId == userId)
                        .Select(wl => wl.SeoUrlKey);

                    return wishLists.Where(wl => seoUrlKeys.Contains(wl.SeoUrlKey)).ToList();
                });
        }
        else
        {
            store
                .Setup(x => x.GetSharedWithUser(It.IsAny<string>()))
                .ReturnsAsync(null as IList<WishList>);
        }
    }
}
