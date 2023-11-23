using System.Text.Json.Serialization;

namespace Egorozh.YandexDisk.Client.Protocol;

/// <summary>
/// Request of moving file on Disk
/// </summary>
public class MoveFileRequest
{
    /// <summary>
    /// Путь к перемещаемому ресурсу. Например, %2Ffoo%2Fphoto.png.
    /// </summary>
    [JsonPropertyName("from")]
    public required string From { get; init; }

    /// <summary>
    /// Путь к новому положению ресурса. Например, %2Fbar%2Fphoto.png.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    /// <summary>
    /// Признак перезаписи. Учитывается, если ресурс копируется в папку, в которой уже есть ресурс с таким именем.
    /// Допустимые значения:
    /// true — удалять файлы с совпадающими именами и записывать копируемые файлы.
    /// false — используется по умолчанию. Указывает не перезаписывать файлы и отменить копирование.
    /// </summary>
    [JsonPropertyName("overwrite")]
    public bool Overwrite { get; set; }
}