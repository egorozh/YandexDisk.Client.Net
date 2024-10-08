using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;


namespace Egorozh.YandexDisk.Client.Http.Progress;


public static class HttpClientFactory
{
    /// <summary>
    /// Creates a new <see cref="HttpClient"/> instance configured with the handlers provided and with an
    /// <see cref="HttpClientHandler"/> as the innermost handler.
    /// </summary>
    /// <param name="handlers">An ordered list of <see cref="DelegatingHandler"/> instances to be invoked as an 
    /// <see cref="HttpRequestMessage"/> travels from the <see cref="HttpClient"/> to the network and an 
    /// <see cref="HttpResponseMessage"/> travels from the network back to <see cref="HttpClient"/>.
    /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
    /// an outbound request message but last for an inbound response message.</param>
    /// <returns>An <see cref="HttpClient"/> instance with the configured handlers.</returns>
    public static HttpClient Create(params DelegatingHandler[] handlers)
    {
        return Create(new HttpClientHandler(), handlers);
    }

    /// <summary>
    /// Creates a new <see cref="HttpClient"/> instance configured with the handlers provided and with the
    /// provided <paramref name="innerHandler"/> as the innermost handler.
    /// </summary>
    /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
    /// <param name="handlers">An ordered list of <see cref="DelegatingHandler"/> instances to be invoked as an 
    /// <see cref="HttpRequestMessage"/> travels from the <see cref="HttpClient"/> to the network and an 
    /// <see cref="HttpResponseMessage"/> travels from the network back to <see cref="HttpClient"/>.
    /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
    /// an outbound request message but last for an inbound response message.</param>
    /// <returns>An <see cref="HttpClient"/> instance with the configured handlers.</returns>
    public static HttpClient Create(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
        HttpMessageHandler pipeline = CreatePipeline(innerHandler, handlers);
        return new HttpClient(pipeline);
    }

    /// <summary>
    /// Creates an instance of an <see cref="HttpMessageHandler"/> using the <see cref="DelegatingHandler"/> instances
    /// provided by <paramref name="handlers"/>. The resulting pipeline can be used to manually create <see cref="HttpClient"/>
    /// or <see cref="HttpMessageInvoker"/> instances with customized message handlers.
    /// </summary>
    /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
    /// <param name="handlers">An ordered list of <see cref="DelegatingHandler"/> instances to be invoked as part 
    /// of sending an <see cref="HttpRequestMessage"/> and receiving an <see cref="HttpResponseMessage"/>.
    /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
    /// an outbound request message but last for an inbound response message.</param>
    /// <returns>The HTTP message channel.</returns>
    public static HttpMessageHandler CreatePipeline(HttpMessageHandler innerHandler, IEnumerable<DelegatingHandler> handlers)
    {
        if (innerHandler == null)
        {
            ArgumentNullException.ThrowIfNull(innerHandler);
        }

        if (handlers == null)
        {
            return innerHandler;
        }

        // Wire handlers up in reverse order starting with the inner handler
        HttpMessageHandler pipeline = innerHandler;
        IEnumerable<DelegatingHandler> reversedHandlers = handlers.Reverse();
        foreach (DelegatingHandler handler in reversedHandlers)
        {
            if (handler == null)
            {
                ArgumentNullException.ThrowIfNull(handlers);
            }

            if (handler.InnerHandler != null)
            {
                ArgumentNullException.ThrowIfNull(handlers);
            }

            handler.InnerHandler = pipeline;
            pipeline = handler;
        }

        return pipeline;
    }
}