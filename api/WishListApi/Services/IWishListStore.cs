using WishListApi.Models.WishList;

namespace WishListApi.Services
{
    public interface IWishListStore
    {
        Task<IList<WishList>> GetForUser(string userId);
        Task<IList<WishList>> GetSharedWithUser(string userId);
        Task Upsert(WishList wishList);
        Task Share(string seoUrlKey, string userId, string sharedWithUserId);
        Task Delete(string seoUrlKey, string userId);
        Task DeleteSharedWishList(string seoUrlKey, string? sharedWithUserId = null);
    }
}
