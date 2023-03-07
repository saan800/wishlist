using WishListApi.Models.WishList;

namespace WishListApi.Services
{
    public interface IWishListService
    {
        /// <summary>
        /// Get all wish lists that the user has access to (owned and shared with)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<WishList>> GetAllForUser(string userId);

        /// <summary>
        /// Get wish list by seoUrlKey if the user has access to it (owned or shared with)
        /// </summary>
        /// <param name="seoUrlKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WishList?> Get(string seoUrlKey, string userId);

        /// <summary>
        /// Create a new wish list for the user. 
        /// The name of the wish list much be unique for the user.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<(WishList? wishList, string? error)> Create(CreateWishListRequest request, string userId);

        /// <summary>
        /// Update wish list for the user. 
        /// The name of the wish list much be unique for the user.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="seoUrlKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<(WishList? wishList, string? error)> Update(UpdateWishListRequest request, string seoUrlKey, string userId);

        /// <summary>
        /// Delete wish list for the user
        /// </summary>
        /// <param name="seoUrlKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task Delete(string seoUrlKey, string userId);
    }
}
