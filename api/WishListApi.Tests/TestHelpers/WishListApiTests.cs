using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WishListApi.Tests.TestHelpers;

public class WishListApiTests : IAsyncLifetime
{
    // TODO: replace MsSql config with DynamoDb
    //      https://github.com/testcontainers/testcontainers-dotnet/blob/develop/examples/WeatherForecast/tests/WeatherForecast.InProcess.Tests/WeatherForecastTest.cs
    // private readonly MsSqlTestcontainer _mssqlContainer;

    public WishListApiTests()
    {
        //        var mssqlConfiguration = new DatabaseContainerConfiguration();
        //        _mssqlContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
        //          .WithDatabase(mssqlConfiguration)
        //          .Build();
    }

    public Task InitializeAsync()
    {
        // return _mssqlContainer.StartAsync();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // return _mssqlContainer.DisposeAsync().AsTask();
        return Task.CompletedTask;
    }

    public abstract class BaseControllerTests : IClassFixture<WishListApiTests>, IDisposable
    {
        protected readonly WebApplicationFactory<LocalEntryPoint> _webApplicationFactory;

        protected readonly IServiceScope _serviceScope;

        protected readonly HttpClient _httpClient;

        protected readonly string _defaultContentType;

        protected BaseControllerTests(WishListApiTests wishListApiTests)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+");
            Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", "certificate.crt");
            Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", "password");

            _webApplicationFactory = new WebApplicationFactory<LocalEntryPoint>();
            // TODO: mock services: https://stackoverflow.com/questions/53507352/replace-service-with-mocked-using-webapplicationfactory

            _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _httpClient = _webApplicationFactory.CreateClient();

            _defaultContentType = "application/json";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_defaultContentType));
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _serviceScope.Dispose();
            _webApplicationFactory.Dispose();
        }

        protected async Task<(HttpStatusCode responseStatusCode, T? responseObj)> Invoke<T>(HttpMethod method, string uri, string body) where T : class
        {
            (HttpStatusCode responseStatusCode, string? responseBody) = await Invoke(method, uri, body);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return string.IsNullOrWhiteSpace(responseBody)
                ? (responseStatusCode, null)
                : (responseStatusCode, JsonSerializer.Deserialize<T>(responseBody, options));

        }

        protected async Task<(HttpStatusCode responseStatusCode, string? responseBody)> Invoke(HttpMethod method, string uri, string body)
        {
            HttpResponseMessage response;
            switch (method.ToString().ToUpper())
            {
                case "GET":
                case "HEAD":
                    // synchronous request without the need for .ContinueWith() or await
                    response = await _httpClient.GetAsync(uri);
                    break;
                case "POST":
                    {
                        // Construct an HttpContent from a StringContent
                        HttpContent content = new StringContent(body);
                        // and add the header to this object instance
                        content.Headers.ContentType = new MediaTypeHeaderValue(_defaultContentType);

                        response = await _httpClient.PostAsync(uri, content);
                    }
                    break;
                case "PUT":
                    {
                        // Construct an HttpContent from a StringContent
                        HttpContent content = new StringContent(body);
                        // and add the header to this object instance
                        // optional: add a formatter option to it as well
                        content.Headers.ContentType = new MediaTypeHeaderValue(_defaultContentType);
                        response = await _httpClient.PutAsync(uri, content);
                    }
                    break;
                case "DELETE":
                    response = await _httpClient.DeleteAsync(uri);
                    break;
                default:
                    throw new NotImplementedException();
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return (response.StatusCode, responseBody);
        }
    }
}
