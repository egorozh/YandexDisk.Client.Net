using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YandexDisk.Client.Http;

/// <summary>
/// Abstract request sender for testing purpose
/// </summary>
public interface IHttpClient: IDisposable
{
    /// <summary>
    /// Send http-request to API
    /// </summary>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken));
}