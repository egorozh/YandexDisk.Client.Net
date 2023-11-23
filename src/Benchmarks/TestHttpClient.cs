using System.Net;
using System.Text;
using YandexDisk.Client.Http;

namespace Benchmarks;

internal class TestHttpClient(HttpStatusCode httpStatusCode = HttpStatusCode.OK, string result = null) : IHttpClient
{
    public static readonly string BaseUrl = "http://ya.ru/api/";
    public static readonly string ApiKey = "test-api-key";


    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var m = new HttpResponseMessage(httpStatusCode)
        {
            Content = new StringContent(result, Encoding.UTF8, "text/json")
        };

        return Task.FromResult(m);
    }

    public void Dispose() { }
}