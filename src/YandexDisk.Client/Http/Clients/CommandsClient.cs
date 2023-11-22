using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients;

internal class CommandsClient(ApiContext apiContext) : DiskClientBase(apiContext), ICommandsClient
{
    public Task<Link> CreateDictionaryAsync(string path, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 1)
        {
            { "path", path }
        };

        return PutAsync(LinkJsonContext.Default.Link, "resources", query, cancellationToken);
    }

    public Task<Link> CopyAsync(CopyFileRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 3)
        {
            { "from", request.From },
            { "path", request.Path },
            { "overwrite", request.Overwrite.ToString().ToLower() }
        };

        return PostAsync(LinkJsonContext.Default.Link, "resources/copy", query, cancellationToken);
    }

    public Task<Link> MoveAsync(MoveFileRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 3)
        {
            { "from", request.From },
            { "path", request.Path },
            { "overwrite", request.Overwrite.ToString().ToLower() }
        };

        return PostAsync(LinkJsonContext.Default.Link, "resources/move", query, cancellationToken);
    }

    public Task<Link> DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 2)
        {
            { "path", request.Path },
            { "permanently", request.Permanently.ToString().ToLower() }
        };

        return DeleteAsync(LinkJsonContext.Default.Link, "resources", query, cancellationToken);
    }

    public Task<Link> EmptyTrashAsync(string path, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 1)
        {
            { "path", path }
        };
        
        return DeleteAsync(LinkJsonContext.Default.Link, "trash/resources", query, cancellationToken);
    }

    public Task<Link> RestoreFromTrashAsync(RestoreFromTrashRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 3)
        {
            { "path", request.Path },
            { "overwrite", request.Overwrite.ToString().ToLower() }
        };
        
        if (!string.IsNullOrWhiteSpace(request.Name))
            query.Add("name", request.Name);
        
        return PutAsync(LinkJsonContext.Default.Link, "trash/resources", query, cancellationToken);
    }

    public async Task<Operation> GetOperationStatus(Link link, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);
        
        Operation operation = await SendAsync(OperationJsonContext.Default.Operation, requestMessage, cancellationToken).ConfigureAwait(false);

        if (operation == null)
        {
            throw new Exception("Unexpected empty result.");
        }

        return operation;
    }
}