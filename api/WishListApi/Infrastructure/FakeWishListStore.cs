using WishListApi.Models.WishList;
using WishListApi.Services;

namespace WishListApi.Infrastructure;

public class FakeWishListStore : IWishListStore
{
    private readonly IList<WishList> _wishListDb;
    private readonly IList<SharedWishList> _sharedWishListDb;

    public FakeWishListStore()
    {
        _wishListDb = new List<WishList>();
        _sharedWishListDb = new List<SharedWishList>();
    }

    public async Task<IList<WishList>> GetForUser(string userId)
    {
        var wishLists = GetWishLists(userId: userId);
        return await Task.FromResult(wishLists);
    }

    public async Task<IList<WishList>> GetSharedWithUser(string userId)
    {
        var seoUrlKeys = _sharedWishListDb
                            .Where(wl => wl.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                            .Select(swl => swl.SeoUrlKey).ToList();
        var wishLists = _wishListDb
                            .Where(wl => seoUrlKeys.Contains(wl.SeoUrlKey));

        return await Task.FromResult(wishLists.ToList());
    }

    public async Task Upsert(WishList wishList)
    {
        var existingWishList = GetWishLists(seoUrlKey: wishList.SeoUrlKey, userId: wishList.UserId).FirstOrDefault();
        if (existingWishList != null)
        {
            await Delete(wishList.SeoUrlKey, wishList.UserId, false);
        }
        _wishListDb.Add(wishList);
    }

    public async Task Share(string seoUrlKey, string userId, string sharedWithUserId)
    {
        var wishList = GetWishLists(seoUrlKey: seoUrlKey, userId: userId).FirstOrDefault();
        if (wishList != null)
        {
            var existingSharedWishList = GetSharedWishLists(seoUrlKey, sharedWithUserId).FirstOrDefault();
            if (existingSharedWishList != null)
            {
                _sharedWishListDb.Add(new SharedWishList { SeoUrlKey = seoUrlKey, UserId = sharedWithUserId });
            }
        }
    }

    public async Task Delete(string seoUrlKey, string userId)
    {
        await Delete(seoUrlKey, userId, true);
    }

    private async Task Delete(string seoUrlKey, string userId, bool deleteShared)
    {
        var wishList = GetWishLists(seoUrlKey: seoUrlKey, userId: userId).FirstOrDefault();
        if (wishList != null)
        {
            _wishListDb.Remove(wishList);

            if (deleteShared)
            {
                await DeleteSharedWishList(seoUrlKey);
            }
        }

        await Task.FromResult(0);
    }

    public async Task DeleteSharedWishList(string seoUrlKey, string? sharedWithUserId = null)
    {
        var existingSharedWishLists = GetSharedWishLists(seoUrlKey, sharedWithUserId);
        foreach (var swl in existingSharedWishLists)
        {
            _sharedWishListDb.Remove(swl);
        }

        await Task.FromResult(0);
    }

    private IList<WishList> GetWishLists(string? seoUrlKey = null, string? userId = null)
        => _wishListDb
                .Where(wl => string.IsNullOrWhiteSpace(seoUrlKey) || wl.SeoUrlKey.Equals(seoUrlKey, StringComparison.OrdinalIgnoreCase))
                .Where(wl => string.IsNullOrWhiteSpace(userId) || wl.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .ToList();

    private IList<SharedWishList> GetSharedWishLists(string? seoUrlKey = null, string? userId = null)
        => _sharedWishListDb
                .Where(swl => string.IsNullOrWhiteSpace(seoUrlKey) || swl.SeoUrlKey.Equals(seoUrlKey, StringComparison.OrdinalIgnoreCase))
                .Where(swl => string.IsNullOrWhiteSpace(userId) || swl.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .ToList();
}
