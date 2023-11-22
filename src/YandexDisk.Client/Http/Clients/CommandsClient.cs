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
    public async Task<Link> CreateDictionaryAsync(string path, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 1)
        {
            { "path", path }
        };

        var response = await PutAsync(HttpObjectType.Json, "resources", query, request: HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Link> CopyAsync(CopyFileRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 3)
        {
            { "from", request.From },
            { "path", request.Path },
            { "overwrite", request.Overwrite.ToString().ToLower() }
        };

        var response = await PostAsync(HttpObjectType.Json, "resources/copy", query, request: HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Link> MoveAsync(MoveFileRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 3)
        {
            { "from", request.From },
            { "path", request.Path },
            { "overwrite", request.Overwrite.ToString().ToLower() }
        };

        var response = await PostAsync(HttpObjectType.Json, "resources/move", query, HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Link> DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 2)
        {
            { "path", request.Path },
            { "permanently", request.Permanently.ToString().ToLower() }
        };

        var response = await DeleteAsync(HttpObjectType.Json, "resources", query, HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Link> EmptyTrashAsync(string path, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 1)
        {
            { "path", path }
        };
        
        var response = await DeleteAsync(HttpObjectType.Json, "trash/resources", query, HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Link> RestoreFromTrashAsync(RestoreFromTrashRequest request,
        CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 3)
        {
            { "path", request.Path },
            { "overwrite", request.Overwrite.ToString().ToLower() }
        };
        
        if (!string.IsNullOrWhiteSpace(request.Name))
            query.Add("name", request.Name);
        
        var response = await PutAsync(HttpObjectType.Json, "trash/resources", query, HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Operation> GetOperationStatus(Link link, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);

        HttpResponseMessage responseMessage =
            await SendAsyncImpl(requestMessage, cancellationToken).ConfigureAwait(false);

        var operationResponse = await ReadResponse(HttpObjectType.Json, responseMessage, cancellationToken)
            .ConfigureAwait(false);

        Operation operation = operationResponse.DeserializeResponse(OperationJsonContext.Default.Operation);

        if (operation == null)
        {
            throw new Exception("Unexpected empty result.");
        }

        return operation;
    }
}