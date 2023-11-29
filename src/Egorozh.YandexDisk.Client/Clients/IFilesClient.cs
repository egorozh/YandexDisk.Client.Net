using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Egorozh.YandexDisk.Client.Protocol;

namespace Egorozh.YandexDisk.Client.Clients;

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
    
    /// <summary>
    /// Download file from Disk on link recivied by <see cref="GetDownloadLinkAsync"/>
    /// without waiting for the file to be completely downloaded to the stream
    /// </summary>
    /// <returns>Stream and content length</returns>
    Task<(Stream, long)> DownloadFastAsync(Link link, CancellationToken cancellationToken = default);


    /// <summary>
    /// Upload file to Disk on link recivied by <see cref="GetUploadLinkAsync"/>
    /// with progress callback 
    /// </summary>
    Task UploadWithProgressAsync(Link link, Stream file, long fileSize, Action<double> progressCallback, CancellationToken ct = default);
}