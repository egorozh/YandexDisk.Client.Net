using System.Web;

namespace Egorozh.YandexDisk.Client.Http;

internal abstract partial class DiskClientBase
{
    protected static string? GetQuery(string key, string? value)
    {
        if (value is null)
            return null;
        
        return $"?{GetQueryKeyValue(key, value)}";
    }


    protected static string? GetQuery(string key1, string? value1, string key2, string? value2)
    {
        string? query = GetQueryImpl(key1, value1, key2, value2);
        
        return query is not null ? $"?{query}" : null;
    }


    protected static string? GetQuery(string key1, string? value1, string key2, string? value2, string key3, string? value3)
    {
        string? query = GetQueryImpl(key1, value1, key2, value2, key3, value3);
        
        return query is not null ? $"?{query}" : null;
    }


    protected static string? GetQuery(
        string key1, string value1, 
        string key2, string? value2, 
        string key3, string? value3,
        string key4, string? value4)
    {
        string? query = GetQueryImpl(key2, value2, key3, value3, key4, value4);
        
        return query is not null ? $"?{GetQueryKeyValue(key1, value1)}&{query}" : null;
    }


    private static string? GetQueryImpl(string key1, string? value1, string key2, string? value2)
    {
        if (value1 is not null && value2 is not null)
            return $"{GetQueryKeyValue(key1, value1)}&{GetQueryKeyValue(key2, value2)}";
        
        if (value1 is null && value2 is not null)
            return $"{GetQueryKeyValue(key2, value2)}";
        
        if (value2 is null && value1 is not null)
            return $"{GetQueryKeyValue(key1, value1)}";
        
        return null;
    }


    private static string? GetQueryImpl(string key1, string? value1, string key2, string? value2, string key3, string? value3)
    {
        if (value1 is not null && value2 is not null && value3 is not null)
            return $"{GetQueryKeyValue(key1, value1)}&{GetQueryKeyValue(key2, value2)}&{GetQueryKeyValue(key3, value3)}";
        
        if (value1 is not null && value3 is not null)
            return $"{GetQueryKeyValue(key1, value1)}&{GetQueryKeyValue(key3, value3)}";
        
        if (value2 is not null && value3 is not null)
            return $"{GetQueryKeyValue(key2, value2)}&{GetQueryKeyValue(key3, value3)}";
        
        if (value1 is null && value2 is null && value3 is not null)
            return $"{GetQueryKeyValue(key3, value3)}";
        
        return GetQueryImpl(key1, value1, key2, value2);
    }


    private static string GetQueryKeyValue(string key, string value) =>
        $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";
}