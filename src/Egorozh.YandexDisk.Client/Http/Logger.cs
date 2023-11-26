using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egorozh.YandexDisk.Client.Http;

internal class Logger(ILogSaver log) : ILogger
{
    private readonly RequestLog _requestLog = new();
    private readonly ResponseLog _responseLog = new();
    private readonly Stopwatch _stopwatch = new();
    

    public async Task SetRequestAsync(HttpRequestMessage request)
    {
        _requestLog.Headers = request.ToString();
        _requestLog.Uri = request.RequestUri?.ToString();
        if (request.Content != null)
        {
            _requestLog.Body = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        _requestLog.StartedAt = DateTime.Now;
        _stopwatch.Start();
    }

    public async Task SetResponseAsync(HttpResponseMessage httpResponseMessage, HttpCompletionOption completionOption)
    {
        _stopwatch.Stop();

        _responseLog.Headers = httpResponseMessage.ToString();
        _responseLog.StatusCode = httpResponseMessage.StatusCode;

        if (httpResponseMessage.Content != null && completionOption == HttpCompletionOption.ResponseContentRead)
        {
            _responseLog.Body = await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        else
        {
            _responseLog.Body = null;
        }
    }

    public void EndWithSuccess()
    {
        _responseLog.Duration = _stopwatch.ElapsedMilliseconds;

        SaveLog();
    }

    public void EndWithError(Exception e)
    {
        _responseLog.Exception = e.Message;
        _responseLog.Duration = _stopwatch.ElapsedMilliseconds;

        SaveLog();
    }

    private void SaveLog()
    {
        log?.SaveLog(_requestLog, _responseLog);

        _isDisposed = true;
    }

    private bool _isDisposed;

    public void Dispose()
    {
        if (!_isDisposed)
        {
            EndWithError(new Exception("Log object is never ended. You should end log befor disposing."));
        }
    }
}