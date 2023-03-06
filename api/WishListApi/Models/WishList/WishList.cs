namespace WishListApi.Models.WishList;

public class WishList
{
    public string SeoUrlKey { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }

    public List<WishListItem> Items { get; set; } = new List<WishListItem>();

    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
}
