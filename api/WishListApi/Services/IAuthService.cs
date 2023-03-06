namespace WishListApi.Services
{
    public interface IAuthService
    {
        string GetJwtToken(string email, string name);
    }
}
