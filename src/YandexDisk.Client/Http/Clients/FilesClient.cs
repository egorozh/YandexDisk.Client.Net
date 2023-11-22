using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients;

internal class FilesClient(ApiContext apiContext) : DiskClientBase(apiContext), IFilesClient
{
    public Task<Link> GetUploadLinkAsync(string path, bool overwrite, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new(capacity: 2)
        {
            { "path", path },
            { "overwrite", overwrite.ToString().ToLower() }
        };

        return GetAsync(LinkJsonContext.Default.Link, "resources/upload", query, cancellationToken);
    }

    public Task UploadAsync(Link link, Stream file, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var content = new StreamContent(file);

        var requestMessage = new HttpRequestMessage(method, url) { Content = content };

        return SendAsyncImpl(requestMessage, cancellationToken);
    }

    public Task<Link> GetDownloadLinkAsync(string path, CancellationToken cancellationToken)
    {
        Dictionary<string, string> query = new(capacity: 1)
        {
            { "path", path }
        };

        return GetAsync(LinkJsonContext.Default.Link, "resources/download", query, cancellationToken);
    }

    public async Task<Stream> DownloadAsync(Link link, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);

        HttpResponseMessage responseMessage = await SendAsyncImpl(requestMessage, cancellationToken).ConfigureAwait(false);

        return await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }
}