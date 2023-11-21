using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http.Serialization;
using YandexDisk.Client.Protocol;

namespace YandexDisk.Client.Http.Clients;

internal class MetaInfoClient : DiskClientBase, IMetaInfoClient
{
    internal MetaInfoClient(ApiContext apiContext)
        : base(apiContext)
    { }

    public async Task<Disk> GetDiskInfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(HttpObjectType.Json, "", (object?) null, cancellationToken);
        
        return response.DeserializeResponse<Disk>(DiskJsonContext.Default);
    }

    public async Task<Resource> GetInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(HttpObjectType.Json,"resources", request, cancellationToken);
        
        return response.DeserializeResponse<Resource>(ResourceJsonContext.Default);
    }

    public async Task<Resource> GetTrashInfoAsync(ResourceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(HttpObjectType.Json,"trash/resources", request, cancellationToken);
        
        return response.DeserializeResponse<Resource>(ResourceJsonContext.Default);
    }

    public async Task<FilesResourceList> GetFilesInfoAsync(FilesResourceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(HttpObjectType.Json,"resources/files", request, cancellationToken);
        
        return response.DeserializeResponse<FilesResourceList>(FilesResourceListJsonContext.Default);
    }

    public async Task<LastUploadedResourceList> GetLastUploadedInfoAsync(LastUploadedResourceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(HttpObjectType.Json,"resources/last-uploaded", request, cancellationToken);
        
        return response.DeserializeResponse<LastUploadedResourceList>(LastUploadedResourceListJsonContext.Default);
    }

    public async Task<Resource> AppendCustomProperties(string path, IDictionary<string, string> customProperties, CancellationToken cancellationToken = default)
    {
        // new { customProperties }
        var request = HttpObject.FromJson(JsonSerializer.Serialize(
            new CustomPropertiesDto { CustomProperties = customProperties },
            CustomPropertiesJsonContext.Default.CustomPropertiesDto));
        
        var response = await PatchAsync(HttpObjectType.Json,"resources", new { path }, request, cancellationToken);
        
        return response.DeserializeResponse<Resource>(ResourceJsonContext.Default);
    }
        
    public async Task<Link> PublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        var response = await PutAsync(HttpObjectType.Json,"resources/publish", new { path }, HttpObject.FromNull(), cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }

    public async Task<Link> UnpublishFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        var response = await PutAsync(HttpObjectType.Json,"resources/unpublish", new { path }, HttpObject.FromNull(), cancellationToken);
        
        return response.DeserializeResponse<Link>(LinkJsonContext.Default);
    }
}


internal class CustomPropertiesDto
{
    public IDictionary<string, string> CustomProperties { get; set; }
}


[JsonSerializable(typeof(CustomPropertiesDto))]
internal partial class CustomPropertiesJsonContext : JsonSerializerContext;