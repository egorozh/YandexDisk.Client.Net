using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YandexDisk.Client.Protocol;

/// <summary>
/// Список ресурсов, содержащихся в папке. Содержит объекты Resource и свойства списка.
/// </summary>
public class ResourceList : ProtocolObjectResponse
{
    /// <summary>
    /// Поле, по которому отсортирован список.
    /// </summary>
    [JsonPropertyName("sort")]
    public string Sort { get; set; }

    /// <summary>
    /// Ключ опубликованной папки, в которой содержатся ресурсы из данного списка.
    /// Включается только в ответ на запрос метаинформации о публичной папке.
    /// </summary>
    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; }

    /// <summary>
    /// Массив ресурсов (Resource), содержащихся в папке.
    /// </summary>
    [JsonPropertyName("items")]
    public List<Resource> Items { get; set; }

    /// <summary>
    /// Путь к папке, чье содержимое описывается в данном объекте ResourceList.
    /// Для публичной папки значение атрибута всегда равно «/».
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; }

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

    /// <summary>
    /// Общее количество ресурсов в папке.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}