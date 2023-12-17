using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Egorozh.YandexDisk.Client.Protocol;

/// <summary>
/// Статус операции. Операции запускаются, когда вы копируете, перемещаете или удаляете непустые папки. 
/// URL для запроса статуса возвращается в ответ на такие запросы.
/// </summary>
public class Operation : ProtocolObjectResponse
{
    /// <summary>
    /// Статус операции
    /// </summary>
    [JsonPropertyName("status")]
    [JsonConverter(typeof(OperationJsonConverter))]
    public OperationStatus Status { get; set; }
}

/// <summary>
/// Возможные статусы опреаций
/// </summary>
public enum OperationStatus
{
    /// <summary>
    /// Операция успешно завершена.
    /// </summary>
    Success,

    /// <summary>
    /// Операцию совершить не удалось, попробуйте повторить изначальный запрос копирования, перемещения или удаления.
    /// </summary>
    Failure,

    /// <summary>
    /// Операция начата, но еще не завершена.
    /// </summary>
    InProgress,
}

internal class OperationJsonConverter : JsonConverter<OperationStatus>
{
    public override OperationStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? stringValue = reader.GetString();

        return stringValue switch
        {
            "success" => OperationStatus.Success,
            "failure" => OperationStatus.Failure,
            "in-progress" => OperationStatus.InProgress,
            _ => OperationStatus.Failure
        };
    }


    public override void Write(Utf8JsonWriter writer, OperationStatus value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}