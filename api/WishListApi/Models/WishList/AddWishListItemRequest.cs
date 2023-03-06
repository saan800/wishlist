using System.ComponentModel.DataAnnotations;

namespace WishListApi.Models.WishList;

public class AddWishListItemRequest
{
    [Required]
    public string Title { get; set; }
}
