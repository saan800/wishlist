namespace WishListApi.Models.WishList;

public class WishListItem
{
    public string Title { get; set; }
    public string AddedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
}
