using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Clients;

/// <summary>
/// Extended file commands
/// </summary>
public static class CommandsClientExtensions
{
    /// <summary>
    /// Default pull period for waiting operation.
    /// </summary>
    public static TimeSpan DefaultPullPeriod = TimeSpan.FromSeconds(3);

    private static async Task WaitOperationAsync(this ICommandsClient client, Link operationLink, CancellationToken cancellationToken, TimeSpan? pullPeriod)
    {
        if (pullPeriod == null)
        {
            pullPeriod = DefaultPullPeriod;
        }

        Operation operation;
        do
        {
            Thread.Sleep(pullPeriod.Value);
            operation = await client.GetOperationStatus(operationLink, cancellationToken).ConfigureAwait(false);
        } while (operation.Status == OperationStatus.InProgress &&
                 !cancellationToken.IsCancellationRequested);
    }

    /// <summary>
    /// Copy file or folder on Disk from one path to another and wait until operation is done
    /// </summary>
    public static async Task CopyAndWaitAsync(this ICommandsClient client, CopyFileRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
    {
        var link = await client.CopyAsync(request, cancellationToken).ConfigureAwait(false);

        if (link.HttpStatusCode == HttpStatusCode.Accepted)
        {
            await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Move file or folder on Disk from one path to another and wait until operation is done
    /// </summary>
    public static async Task MoveAndWaitAsync(this ICommandsClient client, MoveFileRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
    {
        var link = await client.MoveAsync(request, cancellationToken).ConfigureAwait(false);

        if (link.HttpStatusCode == HttpStatusCode.Accepted)
        {
            await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Delete file or folder on Disk and wait until operation is done
    /// </summary>
    public static async Task DeleteAndWaitAsync(this ICommandsClient client, DeleteFileRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
    {
        var link = await client.DeleteAsync(request, cancellationToken).ConfigureAwait(false);

        if (link.HttpStatusCode == HttpStatusCode.Accepted)
        {
            await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Empty trash
    /// </summary>
    public static async Task EmptyTrashAndWaitAsyncAsync(this ICommandsClient client, string path, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
    {
        var link = await client.EmptyTrashAsync(path, cancellationToken).ConfigureAwait(false);

        if (link.HttpStatusCode == HttpStatusCode.Accepted)
        {
            await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Restore files from trash
    /// </summary>
    public static async Task RestoreFromTrashAndWaitAsyncAsync(this ICommandsClient client, RestoreFromTrashRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? pullPeriod = null)
    {
        var link = await client.RestoreFromTrashAsync(request, cancellationToken).ConfigureAwait(false);

        if (link.HttpStatusCode == HttpStatusCode.Accepted)
        {
            await client.WaitOperationAsync(link, cancellationToken, pullPeriod).ConfigureAwait(false);
        }
    }
}