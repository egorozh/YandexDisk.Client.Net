using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egorozh.YandexDisk.Client.Http;

internal interface ILogger: IDisposable
{
    Task SetRequestAsync(HttpRequestMessage request, bool ignoreBody);

    Task SetResponseAsync(HttpResponseMessage httpResponseMessage, HttpCompletionOption completionOption);

    void EndWithSuccess();

    void EndWithError(Exception e);
}