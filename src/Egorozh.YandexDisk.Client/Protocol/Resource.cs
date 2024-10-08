﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Egorozh.YandexDisk.Client.Protocol;

/// <summary>
/// Описание ресурса, мета-информация о файле или папке. Включается в ответ на запрос метаинформации.
/// </summary>
public class Resource : ProtocolObjectResponse
{
    /// <summary>
    /// Ключ опубликованного ресурса.
    /// Включается в ответ только если указанный файл или папка опубликован.
    /// </summary>
    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; }

    /// <summary>
    /// Имя ресурса.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Ссылка на опубликованный ресурс.
    /// Включается в ответ только если указанный файл или папка опубликован.
    /// </summary>
    [JsonPropertyName("public_url")]
    public string PublicUrl { get; set; }

    /// <summary>
    /// Путь к ресурсу до перемещения в Корзину.
    /// Включается в ответ только для запроса метаинформации о ресурсе в Корзине.
    /// </summary>
    [JsonPropertyName("origin_path")]
    public string OriginPath { get; set; }

    /// <summary>
    /// Полный путь к ресурсу на Диске.
    /// В метаинформации опубликованной папки пути указываются относительно самой папки.Для опубликованных файлов значение ключа всегда «/».
    /// Для ресурса, находящегося в Корзине, к атрибуту может быть добавлен уникальный идентификатор(например, trash:/foo_1408546879). С помощью этого идентификатора ресурс можно отличить от других удаленных ресурсов с тем же именем.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; }

    /// <summary>
    /// Ссылка на уменьшенное изображение из файла (превью). Включается в ответ только для файлов поддерживаемых графических форматов.
    /// </summary>
    /// <remarks>Запросить превью можно только с OAuth-токеном пользователя, имеющего доступ к самому файлу.</remarks>
    [JsonPropertyName("preview")]
    public string Preview { get; set; }

    /// <summary>
    /// MD5-хэш файла.
    /// </summary>
    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    /// <summary>
    /// Тип ресурса:
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ResourceType Type { get; set; }

    /// <summary>
    /// MIME-тип файла.
    /// </summary>
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; }

    /// <summary>
    /// Размер файла.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// Дата и время создания ресурса, в формате ISO 8601.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Дата и время изменения ресурса, в формате ISO 8601.
    /// </summary>
    [JsonPropertyName("modified")]
    public DateTimeOffset Modified { get; set; }

    /// <summary>
    /// Объект со всеми атрибутами, заданными с помощью запроса Добавление метаинформации для ресурса. 
    /// Содержит только ключи вида имя:значение (объекты или массивы содержать не может).
    /// </summary>
    [JsonPropertyName("custom_properties")]
    public Dictionary<string, string> CustomProperties { get; set; }

    /// <summary>
    /// Ресурсы, непосредственно содержащиеся в папке (содержит объект ResourceList).
    /// Включается в ответ только при запросе метаинформации о папке.
    /// </summary>
    [JsonPropertyName("_embedded")]
    public ResourceList Embedded { get; set; }
}

/// <summary>
/// Тип ресурсов на Диске
/// </summary>
public enum ResourceType : byte
{
    /// <summary>Папка</summary>
    Dir,

    /// <summary>Файл</summary>
    File
}


[JsonSerializable(typeof(Resource))]
[JsonSourceGenerationOptions(
    UseStringEnumConverter = true, 
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    AllowTrailingCommas = true)]
internal partial class ResourceJsonContext : JsonSerializerContext;