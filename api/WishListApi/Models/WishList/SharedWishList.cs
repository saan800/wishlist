namespace WishListApi.Models.WishList;

public class SharedWishList
{
    public string Id
    {
        get
        {
            return $"{SeoUrlKey}-{UserId}";
        }
    }
    public string SeoUrlKey { get; set; }
    public string UserId { get; set; }
}
