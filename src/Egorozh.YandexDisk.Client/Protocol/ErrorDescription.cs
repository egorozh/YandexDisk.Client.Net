using System.Text.Json.Serialization;

namespace Egorozh.YandexDisk.Client.Protocol;

/// <summary>
/// Дополнительное описание ошибки
/// </summary>
public class ErrorDescription : ProtocolObjectResponse
{
    /// <summary>
    /// Подробное описание ошибки в помощь разработчику.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Идентификатор ошибки для программной обработки.
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; }
}

[JsonSerializable(typeof(ErrorDescription))]
internal partial class ErrorDescriptionJsonContext : JsonSerializerContext;