using WishListApi.Helpers;
using WishListApi.Models.WishList;

namespace WishListApi.Services
{
    public class WishListService : IWishListService
    {
        private readonly IWishListStore _wishListStore;
        private readonly ILogger<WishListService> _logger;

        public WishListService(IWishListStore wishListStore, ILogger<WishListService> logger)
        {
            _wishListStore = wishListStore;
            _logger = logger;
        }

        public async Task<IList<WishList>> GetAllForUser(string userId)
        {
            var wishLists = await _wishListStore.GetForUser(userId) ?? new List<WishList>();

            var sharedWishLists = await _wishListStore.GetSharedWithUser(userId);
            if (sharedWishLists != null)
            {
                wishLists = wishLists.Concat(sharedWishLists).ToList();
            }

            _logger.LogDebug("Found {count} wish lists for {userId}", wishLists.Count, userId);

            return wishLists;
        }

        public async Task<WishList?> Get(string seoUrlKey, string userId)
        {
            var userWishLists = await GetAllForUser(userId);
            return userWishLists.FirstOrDefault(wl => wl.SeoUrlKey.Equals(seoUrlKey, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<(WishList? wishList, string? error)> Create(CreateWishListRequest request, string userId)
        {
            var wishList = new WishList
            {
                SeoUrlKey = WishListHelper.FormatSeoUrlKey(userId, request.Title),
                UserId = userId,
                Title = request.Title,
                CreatedDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            var existingWishList = await Get(wishList.SeoUrlKey, userId);
            if (existingWishList != null)
            {
                _logger.LogInformation("A wish list with the url key {SeoUrlKey} already exists for this user", wishList.SeoUrlKey);
                return (null, $"A wish list with the url key {wishList.SeoUrlKey} already exists for this user");
            }

            await _wishListStore.Upsert(wishList);
            return (wishList, null);
        }

        public async Task<(WishList? wishList, string? error)> Update(UpdateWishListRequest request, string seoUrlKey, string userId)
        {
            var wishList = await Get(seoUrlKey, userId);
            if (wishList == null)
            {
                _logger.LogInformation("Could not find a wish list for {seoUrlKey} for this user", seoUrlKey);
                return (null, $"Could not find a wish list for {seoUrlKey} for this user");
            }

            wishList.Title = request.Title;
            wishList.LastUpdated = DateTime.UtcNow;

            await _wishListStore.Upsert(wishList);
            return (wishList, null);
        }

        public async Task Delete(string seoUrlKey, string userId)
        {
            await _wishListStore.Delete(seoUrlKey, userId);
        }
    }
}
