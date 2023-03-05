using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Xunit;

namespace WishListApi.Tests.TestHelpers;

public static class TestControllerHelper
{
    private const string GetRequestFileName = "./SampleRequests/Controller-Get.json";

    public static APIGatewayProxyRequest BuildGetRequest(string path)
     => BuildGetRequest(path, null);

    public static APIGatewayProxyRequest BuildGetRequest(string path, string? queryString)
     => BuildGetRequest(path, queryString, null);

    public static APIGatewayProxyRequest BuildGetRequest(string path, string? queryString, IDictionary<string, string>? headers)
     => BuildRequest(
            requestFileName: GetRequestFileName,
            httpMethod: "GET",
            path: path,
            queryString: queryString,
            headers: headers,
            body: null
        );

    private static APIGatewayProxyRequest BuildRequest(string requestFileName, string httpMethod, string path, string? queryString, IDictionary<string, string>? headers, string? body)
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

        if (headers != null)
        {
            request.Headers = headers;
        }

        if (!string.IsNullOrWhiteSpace(queryString))
        {
            throw new NotImplementedException($"Need to implement 'queryStringParameters' in {nameof(TestControllerHelper)}");
            //   "queryStringParameters": null,
        }

        if (!string.IsNullOrWhiteSpace(body))
        {
            request.Body = body;
        }

        return request;
    }
}
