using System.Text.Json.Serialization;
using Egorozh.YandexDisk.Client.Protocol;

namespace Egorozh.YandexDisk.Client;

[JsonSerializable(typeof(Disk))]
[JsonSerializable(typeof(ErrorDescription))]
[JsonSerializable(typeof(FilesResourceList))]
[JsonSerializable(typeof(LastUploadedResourceList))]
[JsonSerializable(typeof(Link))]
[JsonSerializable(typeof(Operation))]
[JsonSerializable(typeof(Resource))]
[JsonSourceGenerationOptions(
    UseStringEnumConverter = true, 
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    AllowTrailingCommas = true)]
internal partial class ClientJsonContext : JsonSerializerContext;