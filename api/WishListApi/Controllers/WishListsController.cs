using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WishListApi.Helpers;
using WishListApi.Models.WishList;
using WishListApi.Services;

namespace WishListApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class WishListsController : ControllerBase
{
    private readonly IWishListService _wishListService;

    public WishListsController(IWishListService wishListService)
    {
        _wishListService = wishListService;
    }

    /// <summary>
    /// Get all wish lists the authenticated user has access to, either as the owner or shared with them
    /// </summary>
    /// <returns></returns>
    // GET api/wishlists
    [HttpGet]
    [ProducesResponseType(typeof(IList<WishList>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get()
    {
        var userId = this.User.GetUserId();

        // Get all wishlists for the current user
        return Ok(await _wishListService.GetAllForUser(userId));
    }

    /// <summary>
    /// Get a wish list for the seoUrlKey.
    /// The authenticated user must have access to the wish list, either as the owner or shared with them
    /// </summary>
    /// <param name="seoUrlKey"></param>
    /// <returns></returns>
    // GET api/wishlists/bugs-bunny/birthday
    [HttpGet("{*seoUrlKey}")]
    [ProducesResponseType(typeof(WishList), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get(string seoUrlKey)
    {
        var userId = this.User.GetUserId();
        seoUrlKey = WebUtility.UrlDecode(seoUrlKey);

        var wishList = await _wishListService.Get(seoUrlKey, userId);
        return wishList != null ? Ok(wishList) : NotFound();
    }

    /// <summary>
    /// Create a new wish list for the current user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    // POST api/wishlists
    [HttpPost]
    [ProducesResponseType(typeof(WishList), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> Post([FromBody] CreateWishListRequest request)
    {
        var userId = this.User.GetUserId();
        (WishList? wishList, string? error) = await _wishListService.Create(request, userId);

        return wishList != null ? Created(wishList.SeoUrlKey, wishList) : Conflict(error);
    }

    /// <summary>
    /// Update the details of a wish list.
    /// Only the owner of the wish list can update the main details.
    /// </summary>
    /// <param name="seoUrlKey"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    // PUT api/wishlists/bugs-bunny/birthday
    [HttpPut("{*seoUrlKey}")]
    [ProducesResponseType(typeof(WishList), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> Put(string seoUrlKey, [FromBody] UpdateWishListRequest request)
    {
        var userId = this.User.GetUserId();
        seoUrlKey = WebUtility.UrlDecode(seoUrlKey);

        (WishList? wishList, string? error) = await _wishListService.Update(request, seoUrlKey, userId);

        return wishList != null ? Ok(wishList) : Conflict(error);
    }

    /// <summary>
    /// Delete the wish list.
    /// Only the owner of the wish list can delete it.
    /// </summary>
    /// <param name="seoUrlKey"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    // DELETE api/wishlists/bugs-bunny/birthday
    [HttpDelete("{*seoUrlKey}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(string seoUrlKey)
    {
        var userId = this.User.GetUserId();
        await _wishListService.Delete(seoUrlKey, userId);

        return NoContent();
    }
}
