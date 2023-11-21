using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http.Serialization;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients;

internal class CommandsClient : DiskClientBase, ICommandsClient
{
    internal CommandsClient(ApiContext apiContext)
        : base(apiContext)
    { }
    
    public async Task<Link> CreateDictionaryAsync(string path, CancellationToken cancellationToken = default)
    {
        var response = await PutAsync(HttpObjectType.Json, "resources", new { path }, request: HttpObject.FromNull(), cancellationToken);

        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }
    
    public async Task<Link> CopyAsync(CopyFileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync(HttpObjectType.Json,"resources/copy", request, request: HttpObject.FromNull(), cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public async Task<Link> MoveAsync(MoveFileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync(HttpObjectType.Json,"resources/move", request, HttpObject.FromNull(), cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public async Task<Link> DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await DeleteAsync(HttpObjectType.Json,"resources", request, HttpObject.FromNull(), cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public async Task<Link> EmptyTrashAsync(string path, CancellationToken cancellationToken = default)
    {
        var response = await DeleteAsync(HttpObjectType.Json,"trash/resources", new { path }, HttpObject.FromNull(), cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public async Task<Link> RestoreFromTrashAsync(RestoreFromTrashRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PutAsync(HttpObjectType.Json,"trash/resources", request, HttpObject.FromNull(), cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public async Task<Operation> GetOperationStatus(Link link, CancellationToken cancellationToken = default)
    {
        var url = new Uri(link.Href);

        var method = new HttpMethod(link.Method);

        var requestMessage = new HttpRequestMessage(method, url);

        HttpResponseMessage responseMessage = await SendAsyncImpl(requestMessage, cancellationToken).ConfigureAwait(false);

        var operationResponse = await ReadResponse(HttpObjectType.Json, responseMessage, cancellationToken).ConfigureAwait(false);

        Operation operation = operationResponse.DeserializeResponse<Operation>(OperationJsonContext.Default);
        
        if (operation == null)
        {
            throw new Exception("Unexpected empty result.");
        }

        return operation;
    }
}