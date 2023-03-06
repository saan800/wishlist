using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace WishListApi.Tests.TestHelpers;

public static class TestControllerHelper
{
    private const string GetRequestFileName = "./SampleRequests/Controller-Get.json";
    private const string PostRequestFileName = "./SampleRequests/Controller-Post.json";

    public static APIGatewayProxyRequest BuildGetRequest(string path)
     => BuildGetRequest(path, null);

    public static APIGatewayProxyRequest BuildGetRequest(string path, string? queryString)
     => BuildRequest(
            requestFileName: GetRequestFileName,
            httpMethod: "GET",
            path: path,
            queryString: queryString
        );


    public static APIGatewayProxyRequest BuildPostRequest(string path)
     => BuildPostRequest(path, null);

    public static APIGatewayProxyRequest BuildPostRequest(string path, string? queryString)
     => BuildRequest(
            requestFileName: PostRequestFileName,
            httpMethod: "POST",
            path: path,
            queryString: queryString
            );

    private static APIGatewayProxyRequest BuildRequest(string requestFileName, string httpMethod, string path, string? queryString)
    {
        var requestStr = File.ReadAllText(requestFileName);
        var request = JsonSerializer.Deserialize<APIGatewayProxyRequest>(requestStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (request == null)
        {
            throw new ArgumentException($"Could not create {nameof(APIGatewayProxyRequest)} from {requestFileName}");
        }

        request.Path = !path.StartsWith("/") ? $"/{path}" : path;

        request.PathParameters ??= new Dictionary<string, string> { };
        request.PathParameters.Add("proxy", request.Path[1..]); // remove initial "/" from path

        request.HttpMethod = httpMethod.ToUpper();

        request.RequestContext ??= new APIGatewayProxyRequest.ProxyRequestContext();
        request.RequestContext.HttpMethod = request.HttpMethod;

        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");

        if (!string.IsNullOrWhiteSpace(queryString))
        {
            throw new NotImplementedException($"Need to implement 'queryStringParameters' in {nameof(TestControllerHelper)}");
            //   "queryStringParameters": null,
        }

        return request;
    }

}
