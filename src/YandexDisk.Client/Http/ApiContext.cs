using System;

namespace YandexDisk.Client.Http;

internal class ApiContext(IHttpClient httpClient, Uri baseUrl, ILogSaver? logSaver)
{
    public IHttpClient HttpClient { get; } = httpClient;

    public Uri BaseUrl { get; } = baseUrl;

    public ILogSaver? LogSaver { get; } = logSaver;
}