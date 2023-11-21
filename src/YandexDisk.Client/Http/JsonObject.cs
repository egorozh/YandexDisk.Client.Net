using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;


namespace YandexDisk.Client.Http;


internal readonly struct HttpObject
{
    private readonly HttpObjectType _type;

    private readonly object? _content;

    
    private HttpObject(HttpObjectType type, object? content, HttpStatusCode statusCode)
    {
        _type = type;
        _content = content;
        StatusCode = statusCode;
    }

    
    public HttpStatusCode StatusCode { get; }


    public static HttpContent? ToHttpContent(in HttpObject httpObject)
    {
        if (httpObject._type == HttpObjectType.String)
            return new StringContent((string) httpObject._content);
        if (httpObject._type == HttpObjectType.ByteArray)
            return new ByteArrayContent((byte[]) httpObject._content);
        if (httpObject._type == HttpObjectType.Stream)
            return new StreamContent((Stream) httpObject._content);
        if (httpObject._type == HttpObjectType.Json)
            return new StringContent((string) httpObject._content, Encoding.UTF8, "application/json");
        if (httpObject._type == HttpObjectType.Null)
            return null;

        throw new NotSupportedException($"{nameof(HttpObject)}.{nameof(ToHttpContent)} - request type: {httpObject._content?.GetType()} not supported");
    }

    
    public static HttpObject FromString(string content, HttpStatusCode statusCode = HttpStatusCode.OK) => new(HttpObjectType.String, content, statusCode);
    
    public static HttpObject FromByteArray(byte[] byteArray, HttpStatusCode statusCode = HttpStatusCode.OK) => new(HttpObjectType.ByteArray, byteArray, statusCode);
    
    public static HttpObject FromStream(Stream stream, HttpStatusCode statusCode = HttpStatusCode.OK) => new(HttpObjectType.Stream, stream, statusCode);
    
    public static HttpObject FromJson(string json, HttpStatusCode statusCode = HttpStatusCode.OK) => new(HttpObjectType.Json, json, statusCode);
    
    public static HttpObject FromNull(HttpStatusCode statusCode = HttpStatusCode.OK) => new(HttpObjectType.Null, null, statusCode);


    public string ToStringContent() => (string) _content;
    
    public byte[] ToByteArrayContent() => (byte[]) _content;
    
    public Stream ToStreamContent() => (Stream) _content;
    
    public string ToJsonContent() => (string) _content;

    
    public bool CheckIsNull()
    {
        if (_type is HttpObjectType.Null or HttpObjectType.Unknown)
            return true;

        if (_content is null)
            return true;

        return false;
    }
}


internal enum HttpObjectType
{
    Unknown,
        
    String,
        
    ByteArray,
        
    Stream,
        
    Json,
        
    Null
}