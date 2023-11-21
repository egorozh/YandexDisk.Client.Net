using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients;

/// <summary>
/// Disk file operations
/// </summary>
public interface ICommandsClient
{
    /// <summary>
    /// Create folder on Disk
    /// </summary>
    Task<Link> CreateDictionaryAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Copy fileor folder on Disk from one path to another
    /// </summary>
    Task<Link> CopyAsync(CopyFileRequest request, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Move file or folder on Disk from one path to another
    /// </summary>
    Task<Link> MoveAsync(MoveFileRequest request, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Delete file or folder on Disk
    /// </summary>
    Task<Link> DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Delete files in trash
    /// </summary>
    Task<Link> EmptyTrashAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Delete files in trash
    /// </summary>
    Task<Link> RestoreFromTrashAsync(RestoreFromTrashRequest request, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Return status of operation
    /// </summary>
    Task<Operation> GetOperationStatus(Link link, CancellationToken cancellationToken = default(CancellationToken));
}