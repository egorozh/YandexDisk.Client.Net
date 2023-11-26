using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egorozh.YandexDisk.Client.Http;

internal class DummyLogger : ILogger
{
    public void Dispose()
    { }

    public Task SetRequestAsync(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    public Task SetResponseAsync(HttpResponseMessage httpResponseMessage, HttpCompletionOption completionOption)
    {
        return Task.CompletedTask;
    }

    public void EndWithSuccess()
    { }

    public void EndWithError(Exception e)
    { }
}