using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http.Serialization;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients;

internal class FilesClient : DiskClientBase, IFilesClient
{
    internal FilesClient(ApiContext apiContext)
        : base(apiContext)
    { }

    public async Task<Link> GetUploadLinkAsync(string path, bool overwrite, CancellationToken cancellationToken = default(CancellationToken))
    {
        var response = await GetAsync(HttpObjectType.Json,"resources/upload", new { path, overwrite }, cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public Task UploadAsync(Link link, Stream file, CancellationToken cancellationToken = default(CancellationToken))
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var content = new StreamContent(file);

        var requestMessage = new HttpRequestMessage(method, url) { Content = content };

        return SendAsyncImpl(requestMessage, cancellationToken);
    }

    public async Task<Link> GetDownloadLinkAsync(string path, CancellationToken cancellationToken)
    {
        var response = await GetAsync(HttpObjectType.Json,"resources/download", new { path }, cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public async Task<Stream> DownloadAsync(Link link, CancellationToken cancellationToken = default(CancellationToken))
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);

        HttpResponseMessage responseMessage = await SendAsyncImpl(requestMessage, cancellationToken).ConfigureAwait(false);

        return await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }
}