using System.Text.Json.Serialization;

namespace Egorozh.YandexDisk.Client.Protocol;

/// <summary>
/// Данные о свободном и занятом пространстве на Диск
/// </summary>
public class Disk : ProtocolObjectResponse
{
    /// <summary>
    /// Объем файлов, находящихся в Корзине, в байтах.
    /// </summary>
    [JsonPropertyName("trash_size")]
    public long TrashSize { get; set; }

    /// <summary>
    /// Общий объем Диска, доступный пользователю, в байтах.
    /// </summary>
    [JsonPropertyName("total_space")]
    public long TotalSpace { get; set; }

    /// <summary>
    /// Объем файлов, уже хранящихся на Диске, в байтах.
    /// </summary>
    [JsonPropertyName("used_space")]
    public long UsedSpace { get; set; }

    /// <summary>
    /// Абсолютные адреса системных папок Диска. Имена папок зависят от языка интерфейса пользователя в момент создания персонального Диска. 
    /// Например, для англоязычного пользователя создается папка Downloads, для русскоязычного — Загрузки и т. д.
    /// </summary>
    [JsonPropertyName("system_folders")]
    public SystemFolders SystemFolders { get; set; }
}

/// <summary>
/// Системные папки Диска
/// </summary>
public class SystemFolders
{
    /// <summary>
    /// папка для файлов приложений
    /// </summary>
    [JsonPropertyName("applications")]
    public string Applications { get; set; }

    /// <summary>
    /// папка для файлов, загруженных из интернета(не с устройства пользователя).
    /// </summary>
    [JsonPropertyName("downloads")]
    public string Downloads { get; set; }
}