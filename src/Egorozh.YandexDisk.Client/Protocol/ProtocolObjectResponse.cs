using System.Net;
using System.Text.Json.Serialization;

namespace Egorozh.YandexDisk.Client.Protocol;

/// <summary>
/// Base class of protocol object
/// </summary>
public class ProtocolObjectResponse
{
    /// <summary>
    /// Http status code of response from Yandex Disk API
    /// </summary>
    [JsonPropertyName("http_status_code")]
    public HttpStatusCode HttpStatusCode { get; set; }
}