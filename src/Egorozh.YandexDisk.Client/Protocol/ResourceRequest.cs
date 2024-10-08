﻿using System.Text.Json.Serialization;

namespace Egorozh.YandexDisk.Client.Protocol;

/// <summary>
/// Запрос метаинформации
/// </summary>
public class ResourceRequest
{
    /// <summary>
    /// Атрибут, по которому сортируется список ресурсов, вложенных в папку. В качестве значения можно указывать имена следующих ключей объекта Resource:
    /// name(имя ресурса);
    /// path(путь к ресурсу на Диске);
    /// created(дата создания ресурса);
    /// modified(дата изменения ресурса);
    /// size(размер файла).
    /// Для сортировки в обратном порядке добавьте дефис к значению параметра, например: sort=-name
    /// </summary>
    [JsonPropertyName("sort")]
    public string? Sort { get; set; }

    /// <summary>
    /// Путь к нужному ресурсу относительно корневого каталога Диска. Путь к ресурсу в Корзине следует указывать относительно корневого каталога Корзины.
    /// Путь в значении параметра следует кодировать в URL-формате.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    /// <summary>
    /// Количество ресурсов, вложенных в папку, описание которых следует вернуть в ответе (например, для постраничного вывода).
    /// Значение по умолчанию — 20.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Количество вложенных ресурсов с начала списка, которые следует опустить в ответе (например, для постраничного вывода).
    /// Допустим, папка /foo содержит три файла.Если запросить метаинформацию о папке с параметром offset= 1 и сортировкой по умолчанию, API Диска вернет только описания второго и третьего файла.
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}