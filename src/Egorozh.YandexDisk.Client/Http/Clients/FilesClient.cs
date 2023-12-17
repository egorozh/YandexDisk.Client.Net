using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Egorozh.YandexDisk.Client.Clients;
using Egorozh.YandexDisk.Client.Http.Progress;
using Egorozh.YandexDisk.Client.Protocol;

namespace Egorozh.YandexDisk.Client.Http.Clients;

internal class FilesClient(ApiContext apiContext) : DiskClientBase(apiContext), IFilesClient
{
    public Task<Link> GetUploadLinkAsync(string path, bool overwrite, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("path", path, "overwrite", overwrite.ToString().ToLower());

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
        string? query = GetQuery("path", path);

        return GetAsync(LinkJsonContext.Default.Link, "resources/download", query, cancellationToken);
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


    public async Task<(Stream, long)> DownloadFastAsync(Link link, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);

        HttpResponseMessage responseMessage =
            await SendAsyncImpl(requestMessage, cancellationToken, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

        long contentLength = responseMessage.Content.Headers.ContentLength ?? 0;

        var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        return (stream, contentLength);
    }


    public async Task UploadWithProgressAsync(Link link, Stream file, long fileSize, Action<double>? progressCallback, CancellationToken ct = default)
    {
        using (ILogger logger = LoggerFactory.GetLogger(_logSaver))
        {
            try
            {
                var progress = new ProgressMessageHandler();

                progress.HttpSendProgress += (_, e) =>
                {
                    double progressPercentage = (double)e.BytesTransferred / fileSize;

                    progressCallback?.Invoke(progressPercentage);
                };

                using var client = HttpClientFactory.Create(progress);

                var url = new Uri(link.Href);

                var method = new HttpMethod(link.Method);

                var content = new StreamContent(file);

                var requestMessage = new HttpRequestMessage(method, url) { Content = content };

                await logger.SetRequestAsync(requestMessage, ignoreBody: true).ConfigureAwait(false);


                var response = await client.SendAsync(requestMessage, ct).ConfigureAwait(false);

                await logger.SetResponseAsync(response, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                await EnsureSuccessStatusCode(response).ConfigureAwait(false);

                logger.EndWithSuccess();
            }
            catch (Exception e)
            {
                logger.EndWithError(e);

                throw;
            }
        }
    }
}