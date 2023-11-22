using System.Text.Json.Serialization;

namespace YandexDisk.Client.Protocol;


/// <summary>
/// Request of deleting file on Disk
/// </summary>
public class DeleteFileRequest
{
    /// <summary>
    /// Путь к новому положению ресурса. Например, %2Fbar%2Fphoto.png.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    /// <summary>
    /// Признак безвозвратного удаления. Поддерживаемые значения:
    /// false — удаляемый файл или папка перемещаются в Корзину(используется по умолчанию).
    /// true — файл или папка удаляются без помещения в Корзину.
    /// </summary>
    [JsonPropertyName("permanently")]
    public bool Permanently { get; set; }
}