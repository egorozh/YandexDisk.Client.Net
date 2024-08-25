using System.Threading;
using System.Threading.Tasks;
using Egorozh.YandexDisk.Client.Protocol;

namespace Egorozh.YandexDisk.Client.Clients;

/// <summary>
/// Disk file operations
/// </summary>
public interface ICommandsClient
{
    /// <summary>
    /// Create folder on Disk
    /// </summary>
    Task<Link> CreateDictionaryAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copy fileor folder on Disk from one path to another
    /// </summary>
    Task<Link> CopyAsync(CopyFileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Move file or folder on Disk from one path to another
    /// </summary>
    Task<Link> MoveAsync(MoveFileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete file or folder on Disk
    /// </summary>
    Task<Link?> DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete files in trash
    /// </summary>
    Task<Link> EmptyTrashAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete files in trash
    /// </summary>
    Task<Link> RestoreFromTrashAsync(RestoreFromTrashRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return status of operation
    /// </summary>
    Task<Operation> GetOperationStatus(Link link, CancellationToken cancellationToken = default);
}