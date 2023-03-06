using System.ComponentModel.DataAnnotations;

namespace WishListApi.Models.WishList
{
    public class CreateWishListRequest
    {
        [Required]
        public string Title { get; set; }
    }
}
