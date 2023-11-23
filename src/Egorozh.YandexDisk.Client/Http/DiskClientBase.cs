using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Egorozh.YandexDisk.Client.Protocol;

namespace Egorozh.YandexDisk.Client.Http;


internal abstract partial class DiskClientBase(ApiContext apiContext)
{
    private readonly IHttpClient _httpClient =
        apiContext.HttpClient ?? throw new ArgumentNullException(nameof(apiContext.HttpClient));

    private readonly ILogSaver? _logSaver = apiContext.LogSaver;
    private readonly Uri _baseUrl = apiContext.BaseUrl ?? throw new ArgumentNullException(nameof(apiContext.BaseUrl));
    

    protected Task<T> GetAsync<T>(JsonTypeInfo<T> jsonTypeInfo, string relativeUrl, string? queryRequest, CancellationToken cancellationToken)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(relativeUrl);
        
        Uri url = GetUrl(relativeUrl, queryRequest);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

        return SendAsync(jsonTypeInfo, requestMessage, cancellationToken);
    }


    protected Task<T> PostAsync<T>(JsonTypeInfo<T> jsonTypeInfo, string relativeUrl, string? queryRequest, CancellationToken cancellationToken) 
        where T : new()
    {
        return RequestAsync(jsonTypeInfo, relativeUrl, queryRequest, requestJsonContent: null, HttpMethod.Post, cancellationToken);
    }


    protected Task<T> PutAsync<T>(JsonTypeInfo<T> jsonTypeInfo, string relativeUrl, string? queryRequest, CancellationToken cancellationToken) 
        where T : new()
    {
        return RequestAsync(jsonTypeInfo, relativeUrl, queryRequest, requestJsonContent: null, HttpMethod.Put, cancellationToken);
    }


    protected Task<T> DeleteAsync<T>(JsonTypeInfo<T> jsonTypeInfo, string relativeUrl, string? queryRequest, CancellationToken cancellationToken)
        where T : new()
    {
        return RequestAsync(jsonTypeInfo, relativeUrl, queryRequest, requestJsonContent: null, HttpMethod.Delete, cancellationToken);
    }


    protected Task<T> PatchAsync<T>(JsonTypeInfo<T> jsonTypeInfo, string relativeUrl, string? queryRequest,
        string? requestJsonContent, CancellationToken cancellationToken) where T : new()
    {
        return RequestAsync(jsonTypeInfo, relativeUrl, queryRequest, requestJsonContent, new HttpMethod("PATCH"), cancellationToken);
    }

    
    protected async Task<T> SendAsync<T>(JsonTypeInfo<T> jsonTypeInfo, HttpRequestMessage request, CancellationToken cancellationToken)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpResponseMessage responseMessage = await SendAsyncImpl(request, cancellationToken).ConfigureAwait(false);
        
        string? json = responseMessage.StatusCode != HttpStatusCode.NoContent
            ? await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
            : null;
        
        var result = DeserializeResponse(json, jsonTypeInfo);
        
        //If response is ProtocolObjectResponse,
        //add HttpStatusCode to response
        if (result is ProtocolObjectResponse protocolObject)
        {
            protocolObject.HttpStatusCode = responseMessage.StatusCode;
        }

        return result;
    }
    
    
    protected async Task<HttpResponseMessage> SendAsyncImpl(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        using (ILogger logger = GetLogger())
        {
            await logger.SetRequestAsync(request).ConfigureAwait(false);

            try
            {
                HttpResponseMessage response =
                    await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                await logger.SetResponseAsync(response).ConfigureAwait(false);

                await EnsureSuccessStatusCode(response).ConfigureAwait(false);

                logger.EndWithSuccess();

                return response;
            }
            catch (Exception e)
            {
                logger.EndWithError(e);

                throw;
            }
        }
    }
    

    private ILogger GetLogger() => LoggerFactory.GetLogger(_logSaver);


    private Uri GetUrl(string relativeUrl, string? queryRequest = null)
    {
        var uriBuilder = new UriBuilder(_baseUrl);
        uriBuilder.Path += relativeUrl ?? throw new ArgumentNullException(nameof(relativeUrl));

        if (queryRequest is not null)
        {
            uriBuilder.Query = queryRequest;
        }

        return uriBuilder.Uri;
    }
    
    
    private async Task<T> RequestAsync<T>(JsonTypeInfo<T> jsonTypeInfo, string relativeUrl, string? queryRequest,
        string? requestJsonContent, HttpMethod httpMethod, CancellationToken cancellationToken) where T : new() 
    {
        Uri url = GetUrl(relativeUrl, queryRequest);

        HttpContent? content = !string.IsNullOrWhiteSpace(requestJsonContent) 
            ? new StringContent(requestJsonContent, Encoding.UTF8, "application/json")
            : null;
        
        var requestMessage = new HttpRequestMessage(httpMethod, url) { Content = content };

        return await SendAsync(jsonTypeInfo, requestMessage, cancellationToken);
    }


    private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await TryGetErrorDescriptionAsync(response).ConfigureAwait(false);

            response.Content?.Dispose();

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                throw new NotAuthorizedException(response.ReasonPhrase, error);
            }

            throw new YandexApiException(response.StatusCode, response.ReasonPhrase, error);
        }
    }


    private async Task<ErrorDescription?> TryGetErrorDescriptionAsync(HttpResponseMessage response)
    {
        try
        {
            return response.Content != null
                ? JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                    ErrorDescriptionJsonContext.Default.ErrorDescription)
                : null;
        }
        catch (SerializationException) //unexpected data in content
        {
            return null;
        }
    }
    
    
    private static T DeserializeResponse<T>(string? json, JsonTypeInfo<T> typeInfo) where T : new()
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new Exception($"{nameof(DiskClientBase)}.{nameof(DeserializeResponse)} - response is null");
        
        try
        {
            T result;
            
            //If response body is null but ProtocolObjectResponse was requested, 
            //create empty object
            if (string.IsNullOrWhiteSpace(json) && typeof(ProtocolObjectResponse).IsAssignableFrom(typeof(T)))
            {
                result = new T();
            }
            else
            {
                result = JsonSerializer.Deserialize(json, typeInfo)
                         ?? throw new Exception($"{nameof(DiskClientBase)}.{nameof(DeserializeResponse)} - response is null");;
            }
            
            return result;
        }
        catch (Exception e)
        {
            throw new Exception($"{nameof(DiskClientBase)}.{nameof(DeserializeResponse)} - {e}");
        }
    }
}