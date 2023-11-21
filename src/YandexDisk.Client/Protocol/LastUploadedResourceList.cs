using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YandexDisk.Client.Protocol;

/// <summary>
/// Список последних добавленных на Диск файлов, отсортированных по дате загрузки (от поздних к ранним).
/// </summary>
public class LastUploadedResourceList : ProtocolObjectResponse
{
    /// <summary>
    /// Массив ресурсов (Resource), содержащихся в папке.
    /// </summary>
    [JsonPropertyName("items")]
    public List<Resource> Items { get; set; }

    /// <summary>
    /// Максимальное количество элементов в массиве items, заданное в запросе.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

[JsonSerializable(typeof(LastUploadedResourceList))]
internal partial class LastUploadedResourceListJsonContext : JsonSerializerContext;