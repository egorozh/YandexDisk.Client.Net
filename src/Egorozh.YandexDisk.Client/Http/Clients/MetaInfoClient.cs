using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Egorozh.YandexDisk.Client.Clients;
using Egorozh.YandexDisk.Client.Protocol;

namespace Egorozh.YandexDisk.Client.Http.Clients;

internal class MetaInfoClient(ApiContext apiContext) : DiskClientBase(apiContext), IMetaInfoClient
{
    public Task<Disk> GetDiskInfoAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<Disk>("", null, cancellationToken);
    }

    public Task<Resource> GetInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery(
            key1:"path", request.Path,
            key2: "limit", request.Limit?.ToString(), 
            key3: "offset", request.Offset?.ToString(), 
            key4: "sort", request.Sort);
        
        return GetAsync<Resource>("resources", query, cancellationToken);
    }

    public Task<Resource> GetTrashInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery(
            key1:"path", request.Path,
            key2: "limit", request.Limit?.ToString(), 
            key3: "offset", request.Offset?.ToString(), 
            key4: "sort", request.Sort);
        
        return GetAsync<Resource>("trash/resources", query, cancellationToken);
    }

    public Task<FilesResourceList> GetFilesInfoAsync(FilesResourceRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery(
            key1: "media_type", MediaTypesToString(request.MediaType), 
            key2: "limit", request.Limit?.ToString(), 
            key3: "offset", request.Offset?.ToString());
        
        return GetAsync<FilesResourceList>("resources/files", query, cancellationToken);
    }

    public Task<LastUploadedResourceList> GetLastUploadedInfoAsync(LastUploadedResourceRequest request, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery(
            key1: "media_type", MediaTypesToString(request.MediaType),
            key2: "limit", request.Limit?.ToString());
        
        return GetAsync<LastUploadedResourceList>("resources/last-uploaded", query, cancellationToken);
    }

    public Task<Resource> AppendCustomProperties(string path, IDictionary<string, string> customProperties, CancellationToken cancellationToken = default)
    {
        // new { customProperties }
        string request = JsonSerializer.Serialize(
            new CustomPropertiesDto { CustomProperties = customProperties },
            CustomPropertiesJsonContext.Default.CustomPropertiesDto);

        string? query = GetQuery("path", path);

        return PatchAsync<Resource>("resources", query, request, cancellationToken);
    }

    public Task<Link> PublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("path", path);

        return PutAsync<Link>("resources/publish", query, cancellationToken);
    }

    public Task<Link> UnpublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        string? query = GetQuery("path", path);

        return PutAsync<Link>("resources/unpublish", query, cancellationToken);
    }


    private static string? MediaTypesToString(MediaType[]? mediaTypes) => mediaTypes is not null 
        ? string.Join(",", mediaTypes.Select(t => t.GetName().ToLower())) : null;
}

internal class CustomPropertiesDto
{
    [JsonPropertyName("custom_properties")]
    public IDictionary<string, string> CustomProperties { get; set; }
}

[JsonSerializable(typeof(CustomPropertiesDto))]
internal partial class CustomPropertiesJsonContext : JsonSerializerContext;