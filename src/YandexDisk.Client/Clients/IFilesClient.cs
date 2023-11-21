using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients;

/// <summary>
/// Files operation client
/// </summary>
public interface IFilesClient
{
    /// <summary>
    /// Return link for file upload 
    /// </summary>
    /// <param name="path">Path on Disk for uploading file</param>
    /// <param name="overwrite">If file exists it will be overwritten</param>
    /// <param name="cancellationToken"></param>
    Task<Link> GetUploadLinkAsync(string path, bool overwrite, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload file to Disk on link recivied by <see cref="GetUploadLinkAsync"/>
    /// </summary>
    Task UploadAsync(Link link, Stream file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return link for file download 
    /// </summary>
    /// <param name="path">Path to downloading fileon Disk</param>
    /// <param name="cancellationToken"></param>
    Task<Link> GetDownloadLinkAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download file from Disk on link recivied by <see cref="GetDownloadLinkAsync"/>
    /// </summary>
    Task<Stream> DownloadAsync(Link link, CancellationToken cancellationToken = default);
}