using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients;

internal class MetaInfoClient(ApiContext apiContext) : DiskClientBase(apiContext), IMetaInfoClient
{
    public Task<Disk> GetDiskInfoAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync(DiskJsonContext.Default.Disk, "", null, cancellationToken);
    }

    public Task<Resource> GetInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new(capacity: 4)
        {
            { "path", request.Path }
        };

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        if (request.Offset.HasValue)
            query.Add("offset", request.Offset.Value.ToString());
        
        if (!string.IsNullOrWhiteSpace(request.Sort))
            query.Add("sort", request.Sort);

        return GetAsync(ResourceJsonContext.Default.Resource, "resources", query, cancellationToken);
    }

    public Task<Resource> GetTrashInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new(capacity: 4)
        {
            { "path", request.Path }
        };

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        if (request.Offset.HasValue)
            query.Add("offset", request.Offset.Value.ToString());
        
        if (!string.IsNullOrWhiteSpace(request.Sort))
            query.Add("sort", request.Sort);

        return GetAsync(ResourceJsonContext.Default.Resource, "trash/resources", query, cancellationToken);
    }

    public Task<FilesResourceList> GetFilesInfoAsync(FilesResourceRequest request, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new(capacity: 3);
        
        if (request.MediaType is not null && request.MediaType.Length > 0)
            query.Add("media_type", MediaTypesToString(request.MediaType));

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        if (request.Offset.HasValue)
            query.Add("offset", request.Offset.Value.ToString());

        return GetAsync(FilesResourceListJsonContext.Default.FilesResourceList, "resources/files", query, cancellationToken);
    }

    public Task<LastUploadedResourceList> GetLastUploadedInfoAsync(LastUploadedResourceRequest request, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new(capacity: 2);
        
        if (request.MediaType is not null && request.MediaType.Length > 0)
            query.Add("media_type", MediaTypesToString(request.MediaType));

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        return GetAsync(LastUploadedResourceListJsonContext.Default.LastUploadedResourceList, "resources/last-uploaded", query, cancellationToken);
    }

    public Task<Resource> AppendCustomProperties(string path, IDictionary<string, string> customProperties, CancellationToken cancellationToken = default)
    {
        // new { customProperties }
        string request = JsonSerializer.Serialize(
            new CustomPropertiesDto { CustomProperties = customProperties },
            CustomPropertiesJsonContext.Default.CustomPropertiesDto);

        Dictionary<string, string> query = new(capacity: 1)
        {
            { "path", $"{path}" }
        };

        return PatchAsync(ResourceJsonContext.Default.Resource, "resources", query, request, cancellationToken);
    }

    public Task<Link> PublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new(capacity: 1)
        {
            { "path", $"{path}" }
        };

        return PutAsync(LinkJsonContext.Default.Link, "resources/publish", query, cancellationToken);
    }

    public Task<Link> UnpublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new(capacity: 1)
        {
            { "path", $"{path}" }
        };

        return PutAsync(LinkJsonContext.Default.Link, "resources/unpublish", query, cancellationToken);
    }


    private static string MediaTypesToString(MediaType[] mediaTypes) => string.Join(",", mediaTypes.Select(t => t.GetName().ToLower()));
}

internal class CustomPropertiesDto
{
    [JsonPropertyName("custom_properties")]
    public IDictionary<string, string> CustomProperties { get; set; }
}

[JsonSerializable(typeof(CustomPropertiesDto))]
internal partial class CustomPropertiesJsonContext : JsonSerializerContext;