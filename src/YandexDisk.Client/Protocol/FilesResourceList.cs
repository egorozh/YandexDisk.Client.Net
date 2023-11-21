using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YandexDisk.Client.Protocol;

/// <summary>
/// Плоский список всех файлов на Диске в алфавитном порядке.
/// </summary>
public class FilesResourceList : ProtocolObjectResponse
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

    /// <summary>
    /// Смещение начала списка от первого ресурса в папке.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}

[JsonSerializable(typeof(FilesResourceList))]
[JsonSourceGenerationOptions()]
internal partial class FilesResourceListJsonContext : JsonSerializerContext;