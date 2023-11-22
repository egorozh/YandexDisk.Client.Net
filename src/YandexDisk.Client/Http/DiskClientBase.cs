using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using YandexDisk.Client.Protocol;


namespace YandexDisk.Client.Http;


internal abstract class DiskClientBase(ApiContext apiContext)
{
    private readonly IHttpClient _httpClient =
        apiContext.HttpClient ?? throw new ArgumentNullException(nameof(apiContext.HttpClient));

    private readonly ILogSaver? _logSaver = apiContext.LogSaver;
    private readonly Uri _baseUrl = apiContext.BaseUrl ?? throw new ArgumentNullException(nameof(apiContext.BaseUrl));
    
    
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

        throw new NotSupportedException(
            $"{nameof(DiskClientBase)}.{nameof(ReadResponse)} - responseType type: {responseType.GetName()} not supported");
    }


    protected Task<HttpObject> GetAsync(HttpObjectType responseType, string relativeUrl, NameValueCollection? queryRequest, CancellationToken cancellationToken)
    {
        if (relativeUrl == null)
        {
            throw new ArgumentNullException(nameof(relativeUrl));
        }

        Uri url = GetUrl(relativeUrl, queryRequest);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

        return SendAsync(responseType, requestMessage, cancellationToken);
    }


    protected Task<HttpObject> PostAsync(HttpObjectType responseType, string relativeUrl, NameValueCollection? queryRequest,
        HttpObject request,
        CancellationToken cancellationToken)
    {
        return RequestAsync(responseType, relativeUrl, queryRequest, request, HttpMethod.Post, cancellationToken);
    }


    protected Task<HttpObject> PutAsync(HttpObjectType responseType, string relativeUrl, NameValueCollection? queryRequest,
        HttpObject request,
        CancellationToken cancellationToken)
    {
        return RequestAsync(responseType, relativeUrl, queryRequest, request, HttpMethod.Put, cancellationToken);
    }


    protected Task<HttpObject> DeleteAsync(HttpObjectType responseType, string relativeUrl,
        NameValueCollection? queryRequest, HttpObject request,
        CancellationToken cancellationToken)
    {
        return RequestAsync(responseType, relativeUrl, queryRequest, request, HttpMethod.Delete, cancellationToken);
    }


    protected Task<HttpObject> PatchAsync(HttpObjectType responseType, string relativeUrl, NameValueCollection? queryRequest,
        HttpObject request,
        CancellationToken cancellationToken)
    {
        return RequestAsync(responseType, relativeUrl, queryRequest, request, new HttpMethod("PATCH"), cancellationToken);
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


    private Uri GetUrl(string relativeUrl, NameValueCollection? queryRequest = null)
    {
        var uriBuilder = new UriBuilder(_baseUrl);
        uriBuilder.Path += relativeUrl ?? throw new ArgumentNullException(nameof(relativeUrl));

        if (queryRequest is not null)
        {
            uriBuilder.Query = ToQueryString(queryRequest);
        }

        return uriBuilder.Uri;
    }
    
    private static string ToQueryString(NameValueCollection nvc)
    {
        var array = (
            from key in nvc.AllKeys
            from value in nvc.GetValues(key)
            select string.Format(
                "{0}={1}",
                HttpUtility.UrlEncode(key),
                HttpUtility.UrlEncode(value))
        ).ToArray();
        return "?" + string.Join('&', array);
    }


    private async Task<HttpObject> SendAsync(HttpObjectType responseType, HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpResponseMessage responseMessage = await SendAsyncImpl(request, cancellationToken).ConfigureAwait(false);

        var response = await ReadResponse(responseType, responseMessage, cancellationToken).ConfigureAwait(false);

        return response;
    }


    private Task<HttpObject> RequestAsync(HttpObjectType responseType, string relativeUrl, NameValueCollection? queryRequest,
        in HttpObject request, HttpMethod httpMethod, CancellationToken cancellationToken)
    {
        Uri url = GetUrl(relativeUrl, queryRequest);

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
                ? JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                    ErrorDescriptionJsonContext.Default.ErrorDescription)
                : null;
        }
        catch (SerializationException) //unexpected data in content
        {
            return null;
        }
    }
}