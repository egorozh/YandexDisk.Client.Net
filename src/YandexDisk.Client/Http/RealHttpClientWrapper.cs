using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YandexDisk.Client.Http;

internal class RealHttpClientWrapper(HttpMessageInvoker httpMessageInvoker): IHttpClient
{
    private HttpMessageInvoker HttpMessageInvoker { get; } = httpMessageInvoker;


    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return HttpMessageInvoker.SendAsync(request, cancellationToken);
    }

    public void Dispose() => HttpMessageInvoker.Dispose();
}