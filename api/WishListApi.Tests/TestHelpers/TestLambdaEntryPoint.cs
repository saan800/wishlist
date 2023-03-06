using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WishListApi.Tests.TestHelpers;

public class TestLambdaEntryPoint : LambdaEntryPoint
{
    private readonly IReadOnlyCollection<ServiceOverride> _serviceOverrides;

    public TestLambdaEntryPoint() : base()
    {
    }

    public TestLambdaEntryPoint(IReadOnlyCollection<ServiceOverride> serviceOverrides)
    {
        _serviceOverrides = serviceOverrides;
    }

    protected override void Init(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile("appsettings.Development.json");

        });
        builder.UseStartup<Startup>();

        // TODO: this doesn't work!!!! Looks like this is being called before the constructor ;(
        if (_serviceOverrides != null)
        {
            builder.ConfigureServices(services =>
            {
                foreach (var sco in _serviceOverrides)
                {
                    services.Replace(sco);
                }
            });
        }
    }
}

public class ServiceOverride
{
    public ServiceOverride(Type serviceType, Type implementationType)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
    }

    public ServiceOverride(Type serviceType, object objInstance)
    {
        ServiceType = serviceType;
        ObjInstance = objInstance;
    }

    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public object ObjInstance { get; }
}

public static class TestStartupExtensions
{
    public static void Replace(this IServiceCollection services, ServiceOverride so)
    {
        if (so.ObjInstance != null)
        {
            services.Replace(new ServiceDescriptor(so.ServiceType, so.ObjInstance));
        }
        else
        {
            var serviceLifeTime = services.FirstOrDefault(s => s.ServiceType == so.ServiceType)?.Lifetime ?? ServiceLifetime.Transient;

            var descriptor = new ServiceDescriptor(so.ServiceType, so.ImplementationType, serviceLifeTime);
            services.Replace(descriptor);
        }
    }
}
