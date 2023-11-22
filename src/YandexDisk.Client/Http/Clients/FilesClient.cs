using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients;

internal class FilesClient(ApiContext apiContext) : DiskClientBase(apiContext), IFilesClient
{
    public async Task<Link> GetUploadLinkAsync(string path, bool overwrite,
        CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 2)
        {
            { "path", path },
            { "overwrite", overwrite.ToString().ToLower() }
        };

        var response = await GetAsync(HttpObjectType.Json, "resources/upload", query, cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public Task UploadAsync(Link link, Stream file, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var content = new StreamContent(file);

        var requestMessage = new HttpRequestMessage(method, url) { Content = content };

        return SendAsyncImpl(requestMessage, cancellationToken);
    }

    public async Task<Link> GetDownloadLinkAsync(string path, CancellationToken cancellationToken)
    {
        NameValueCollection query = new(capacity: 1)
        {
            { "path", path }
        };

        var response = await GetAsync(HttpObjectType.Json, "resources/download", query, cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Stream> DownloadAsync(Link link, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);

        HttpResponseMessage responseMessage =
            await SendAsyncImpl(requestMessage, cancellationToken).ConfigureAwait(false);

        return await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }
}