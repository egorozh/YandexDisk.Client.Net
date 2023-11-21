using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Http.Serialization;
using YandexDisk.Client.Protocol;


namespace YandexDisk.Client.Http;


internal abstract class DiskClientBase(ApiContext apiContext)
{
    private readonly IHttpClient _httpClient =
        apiContext.HttpClient ?? throw new ArgumentNullException(nameof(apiContext.HttpClient));

    private readonly ILogSaver? _logSaver = apiContext.LogSaver;
    private readonly Uri _baseUrl = apiContext.BaseUrl ?? throw new ArgumentNullException(nameof(apiContext.BaseUrl));

    private static readonly QueryParamsSerializer MvcSerializer = new();
    

    protected async Task<HttpObject> ReadResponse(HttpObjectType responseType, HttpResponseMessage responseMessage,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(responseMessage);

        if (responseMessage.StatusCode == HttpStatusCode.NoContent)
            return HttpObject.FromNull(responseMessage.StatusCode);

        if (responseType == HttpObjectType.String)
            return HttpObject.FromString(await responseMessage.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false), responseMessage.StatusCode);
        if (responseType == HttpObjectType.ByteArray)
            return HttpObject.FromByteArray(await responseMessage.Content.ReadAsByteArrayAsync(cancellationToken)
                .ConfigureAwait(false), responseMessage.StatusCode);
        if (responseType == HttpObjectType.Stream)
            return HttpObject.FromStream(await responseMessage.Content.ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false), responseMessage.StatusCode);
        if (responseType == HttpObjectType.Json)
            return HttpObject.FromJson(await responseMessage.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false), responseMessage.StatusCode);

        throw new NotSupportedException($"{nameof(DiskClientBase)}.{nameof(ReadResponse)} - responseType type: {responseType} not supported");
    }


    protected Task<HttpObject> GetAsync<TParams>(HttpObjectType responseType, string relativeUrl, TParams? parameters,
        CancellationToken cancellationToken)
        where TParams : class
    {
        if (relativeUrl == null)
        {
            throw new ArgumentNullException(nameof(relativeUrl));
        }

        Uri url = GetUrl(relativeUrl, parameters);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

        return SendAsync(responseType, requestMessage, cancellationToken);
    }


    protected Task<HttpObject> PostAsync<TParams>(HttpObjectType responseType, string relativeUrl, TParams? parameters, HttpObject request,
        CancellationToken cancellationToken)
        where TParams : class
    {
        return RequestAsync(responseType, relativeUrl, parameters, request, HttpMethod.Post, cancellationToken);
    }


    protected Task<HttpObject> PutAsync<TParams>(HttpObjectType responseType, string relativeUrl, TParams? parameters, HttpObject request,
        CancellationToken cancellationToken)
        where TParams : class
    {
        return RequestAsync(responseType, relativeUrl, parameters, request, HttpMethod.Put, cancellationToken);
    }


    protected Task<HttpObject> DeleteAsync<TParams>(HttpObjectType responseType, string relativeUrl, TParams? parameters, HttpObject request,
        CancellationToken cancellationToken)
        where TParams : class
    {
        return RequestAsync(responseType, relativeUrl, parameters, request, HttpMethod.Delete, cancellationToken);
    }


    protected Task<HttpObject> PatchAsync<TParams>(HttpObjectType responseType, string relativeUrl, TParams? parameters, HttpObject request,
        CancellationToken cancellationToken)
        where TParams : class
    {
        return RequestAsync(responseType, relativeUrl, parameters, request, new HttpMethod("PATCH"), cancellationToken);
    }

    
    
    protected async Task<HttpResponseMessage> SendAsyncImpl(HttpRequestMessage request,
        CancellationToken cancellationToken)
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


    private Uri GetUrl(string relativeUrl, object? request = null)
    {
        var uriBuilder = new UriBuilder(_baseUrl);
        uriBuilder.Path += relativeUrl ?? throw new ArgumentNullException(nameof(relativeUrl));

        if (request != null)
        {
            uriBuilder.Query = MvcSerializer.Serialize(request);
        }

        return uriBuilder.Uri;
    }


    private async Task<HttpObject> SendAsync(HttpObjectType responseType, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpResponseMessage responseMessage = await SendAsyncImpl(request, cancellationToken).ConfigureAwait(false);

        var response = await ReadResponse(responseType, responseMessage, cancellationToken).ConfigureAwait(false);

        return response;
    }


    private Task<HttpObject> RequestAsync<TParams>(HttpObjectType responseType, string relativeUrl, TParams? parameters, in HttpObject request,
        HttpMethod httpMethod, CancellationToken cancellationToken)
        where TParams : class
    {
        Uri url = GetUrl(relativeUrl, parameters);

        HttpContent? content = HttpObject.ToHttpContent(request);

        var requestMessage = new HttpRequestMessage(httpMethod, url) { Content = content };

        return SendAsync(responseType, requestMessage, cancellationToken);
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
                ? JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync().ConfigureAwait(false), ErrorDescriptionJsonContext.Default.ErrorDescription)
                : null;
        }
        catch (SerializationException) //unexpected data in content
        {
            return null;
        }
    }
}