using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Egorozh.YandexDisk.Client.Http;

internal class RealHttpClientWrapper(HttpClient httpClient) : IHttpClient
{
    public Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken, 
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead) 
        => httpClient.SendAsync(request, completionOption, cancellationToken);

    public void Dispose() => httpClient.Dispose();
}