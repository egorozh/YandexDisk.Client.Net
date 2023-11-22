using System.Collections.Generic;
using System.Collections.Specialized;
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
    public async Task<Disk> GetDiskInfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(HttpObjectType.Json, "", null, cancellationToken);

        return response.DeserializeResponse(DiskJsonContext.Default.Disk);
    }

    public async Task<Resource> GetInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 4)
        {
            { "path", request.Path }
        };

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        if (request.Offset.HasValue)
            query.Add("offset", request.Offset.Value.ToString());
        
        if (!string.IsNullOrWhiteSpace(request.Sort))
            query.Add("sort", request.Sort);

        var response = await GetAsync(HttpObjectType.Json, "resources", query, cancellationToken);

        return response.DeserializeResponse(ResourceJsonContext.Default.Resource);
    }

    public async Task<Resource> GetTrashInfoAsync(ResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 4)
        {
            { "path", request.Path }
        };

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        if (request.Offset.HasValue)
            query.Add("offset", request.Offset.Value.ToString());
        
        if (!string.IsNullOrWhiteSpace(request.Sort))
            query.Add("sort", request.Sort);

        var response = await GetAsync(HttpObjectType.Json, "trash/resources", query, cancellationToken);

        return response.DeserializeResponse(ResourceJsonContext.Default.Resource);
    }

    public async Task<FilesResourceList> GetFilesInfoAsync(FilesResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 3);
        
        if (request.MediaType is not null && request.MediaType.Length > 0)
            query.Add("media_type", MediaTypesToString(request.MediaType));

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        if (request.Offset.HasValue)
            query.Add("offset", request.Offset.Value.ToString());

        var response = await GetAsync(HttpObjectType.Json, "resources/files", query, cancellationToken);

        return response.DeserializeResponse(FilesResourceListJsonContext.Default.FilesResourceList);
    }

    public async Task<LastUploadedResourceList> GetLastUploadedInfoAsync(LastUploadedResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 2);
        
        if (request.MediaType is not null && request.MediaType.Length > 0)
            query.Add("media_type", MediaTypesToString(request.MediaType));

        if (request.Limit.HasValue)
            query.Add("limit", request.Limit.Value.ToString());

        var response = await GetAsync(HttpObjectType.Json, "resources/last-uploaded", query, cancellationToken);

        return response.DeserializeResponse(LastUploadedResourceListJsonContext.Default.LastUploadedResourceList);
    }

    public async Task<Resource> AppendCustomProperties(string path, IDictionary<string, string> customProperties,
        CancellationToken cancellationToken = default)
    {
        // new { customProperties }
        var request = HttpObject.FromJson(JsonSerializer.Serialize(
            new CustomPropertiesDto { CustomProperties = customProperties },
            CustomPropertiesJsonContext.Default.CustomPropertiesDto));

        NameValueCollection query = new(capacity: 1)
        {
            { "path", $"{path}" }
        };

        var response = await PatchAsync(HttpObjectType.Json, "resources", query, request, cancellationToken);

        return response.DeserializeResponse(ResourceJsonContext.Default.Resource);
    }

    public async Task<Link> PublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 1)
        {
            { "path", $"{path}" }
        };

        var response = await PutAsync(HttpObjectType.Json, "resources/publish", query, HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }

    public async Task<Link> UnpublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        NameValueCollection query = new(capacity: 1)
        {
            { "path", $"{path}" }
        };

        var response = await PutAsync(HttpObjectType.Json, "resources/unpublish", query, HttpObject.FromNull(),
            cancellationToken);

        return response.DeserializeResponse(LinkJsonContext.Default.Link);
    }


    private static string MediaTypesToString(MediaType[] mediaTypes)
    {
        return string.Join(",", mediaTypes.Select(t => t.GetName().ToLower()));
    }
}

internal class CustomPropertiesDto
{
    [JsonPropertyName("custom_properties")]
    public IDictionary<string, string> CustomProperties { get; set; }
}

[JsonSerializable(typeof(CustomPropertiesDto))]
internal partial class CustomPropertiesJsonContext : JsonSerializerContext;