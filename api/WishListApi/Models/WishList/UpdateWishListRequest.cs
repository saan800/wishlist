using System.ComponentModel.DataAnnotations;

namespace WishListApi.Models.WishList
{
    public class UpdateWishListRequest
    {
        [Required]
        public string Title { get; set; }
    }
}
