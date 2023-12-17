using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Egorozh.YandexDisk.Client.Clients;
using Egorozh.YandexDisk.Client.Protocol;

namespace Egorozh.YandexDisk.Client.Http.Clients;

internal class CommandsClient(ApiContext apiContext) : DiskClientBase(apiContext), ICommandsClient
{
    public Task<Link> CreateDictionaryAsync(string path, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("path", path);

        return PutAsync<Link>("resources", query, cancellationToken);
    }
    
    public Task<Link> CopyAsync(CopyFileRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("from", request.From, "path", request.Path, "overwrite", request.Overwrite.ToString().ToLower());
        
        return PostAsync<Link>("resources/copy", query, cancellationToken);
    }

    public Task<Link> MoveAsync(MoveFileRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("from", request.From, "path", request.Path, "overwrite", request.Overwrite.ToString().ToLower());
        
        return PostAsync<Link>("resources/move", query, cancellationToken);
    }

    public Task<Link> DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("path", request.Path, "permanently", request.Permanently.ToString().ToLower());
        
        return DeleteAsync<Link>("resources", query, cancellationToken);
    }

    public Task<Link> EmptyTrashAsync(string path, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("path", path);
        
        return DeleteAsync<Link>("trash/resources", query, cancellationToken);
    }

    public Task<Link> RestoreFromTrashAsync(RestoreFromTrashRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("path", request.Path, "overwrite", request.Overwrite.ToString().ToLower(), "name", request.Name);
        
        return PutAsync<Link>("trash/resources", query, cancellationToken);
    }

    public async Task<Operation> GetOperationStatus(Link link, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);
        
        Operation operation = await SendAsync<Operation>(requestMessage, cancellationToken).ConfigureAwait(false);

        if (operation == null)
        {
            throw new Exception("Unexpected empty result.");
        }

        return operation;
    }
}