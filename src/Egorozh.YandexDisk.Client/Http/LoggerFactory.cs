namespace Egorozh.YandexDisk.Client.Http;

internal static class LoggerFactory
{
    public static ILogger GetLogger(ILogSaver? saver) => saver != null ? new Logger(saver) : new DummyLogger();
}