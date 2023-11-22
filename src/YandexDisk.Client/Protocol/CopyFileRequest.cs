using System.Text.Json.Serialization;

namespace YandexDisk.Client.Protocol;

/// <summary>
/// Request of coping file on Disk
/// </summary>
public class CopyFileRequest
{
    /// <summary>
    /// Путь к копируемому ресурсу. Например, %2Ffoo%2Fphoto.png.
    /// </summary>
    [JsonPropertyName("from")]
    public required string From { get; init; }

    /// <summary>
    /// Путь к создаваемой копии ресурса. Например, %2Fbar%2Fphoto.png.
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