namespace WishListApi.Tests.TestHelpers
{
    public class TestLambdaEntryPoint : LambdaEntryPoint
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.Development.json");

            });
            base.Init(builder);
        }
    }
}
