using System;
using System.Net;

namespace Egorozh.YandexDisk.Client.Http;

/// <summary>
/// Logger noticed on each request-response API operation.
/// </summary>
public interface ILogSaver
{
    /// <summary>
    /// Trigered on each request
    /// </summary>
    void SaveLog(RequestLog requestLog, ResponseLog responseLog);
}

/// <summary>
/// Describe output data of http request 
/// </summary>
public class RequestLog
{
    /// <summary>
    /// Request Url
    /// </summary>
    public string Uri { get; set; }

    /// <summary>
    /// Sent headers to Yandex
    /// </summary>
    public string Headers { get; set; }

    /// <summary>
    /// Sent request body to Yandex
    /// </summary>
    public byte[] Body { get; set; }

    /// <summary>
    /// Time of request started
    /// </summary>
    public DateTime StartedAt { get; set; }
}

/// <summary>
/// Describe input data of http request 
/// </summary>
public class ResponseLog
{
    /// <summary>
    /// Response body from Yandex
    /// </summary>
    public byte[]? Body { get; set; }

    /// <summary>
    /// Response headers from Yandex
    /// </summary>
    public string Headers { get; set; }

    /// <summary>
    /// Exception text if it have been risen
    /// </summary>
    public string Exception { get; set; }

    /// <summary>
    /// Http status code from Yandex
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Request-response time duration in milliseconds
    /// </summary>
    public long Duration { get; set; }
}