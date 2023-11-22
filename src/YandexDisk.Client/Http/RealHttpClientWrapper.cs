using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YandexDisk.Client.Http;

internal class RealHttpClientWrapper(HttpMessageInvoker httpMessageInvoker) : IHttpClient
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) 
        => httpMessageInvoker.SendAsync(request, cancellationToken);

    public void Dispose() => httpMessageInvoker.Dispose();
}