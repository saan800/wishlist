using WishListApi;

var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

var host = builder.Build();
host.Run();

#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable CA1050 // Declare types in namespaces
public class LocalEntryPoint
{
}
#pragma warning restore CA1050 // Declare types in namespaces
#pragma warning restore S3903 // Types should be defined in named namespaces
