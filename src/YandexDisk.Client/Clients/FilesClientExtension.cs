using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients;

/// <summary>
/// Extended helpers from uploading and downloading files
/// </summary>
public static class FilesClientExtension
{
    /// <summary>
    /// Just upload stream data to Yandex Disk
    /// </summary>
    public static async Task UploadFileAsync( this IFilesClient client,  string path, bool overwrite,  Stream file, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }
        if (String.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        Link link = await client.GetUploadLinkAsync(path, overwrite, cancellationToken).ConfigureAwait(false);

        await client.UploadAsync(link, file, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Just upload file from local disk to Yandex Disk
    /// </summary>
    public static async Task UploadFileAsync( this IFilesClient client,  string path, bool overwrite,  string localFile, CancellationToken cancellationToken)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }
        if (String.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (String.IsNullOrWhiteSpace(localFile))
        {
            throw new ArgumentNullException(nameof(localFile));
        }

        Link link = await client.GetUploadLinkAsync(path, overwrite, cancellationToken).ConfigureAwait(false);

        using (var file = new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            await client.UploadAsync(link, file, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Get downloaded file from Yandex Disk as stream
    /// </summary>
    public static async Task<Stream> DownloadFileAsync( this IFilesClient client,  string path, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }
        if (String.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        Link link = await client.GetDownloadLinkAsync(path, cancellationToken).ConfigureAwait(false);

        return await client.DownloadAsync(link, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloaded data from Yandex Disk to local file
    /// </summary>
    public static async Task DownloadFileAsync( this IFilesClient client,  string path,  string localFile, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }
        if (String.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (String.IsNullOrWhiteSpace(localFile))
        {
            throw new ArgumentNullException(nameof(localFile));
        }

        Stream data = await DownloadFileAsync(client, path, cancellationToken).ConfigureAwait(false);

        using (var file = new FileStream(localFile, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
        {
            await data.CopyToAsync(file, bufferSize: 81920/*keep default*/, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}