using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Egorozh.YandexDisk.Client.Http;

internal abstract partial class DiskClientBase
{
    protected static string GetQuery(string key, string value) => $"?{GetQueryKeyValue(key, value)}";

    protected static string GetQuery(string key1, string value1, string key2, string value2) =>
        $"?{GetQueryKeyValue(key1, value1)}&{GetQueryKeyValue(key2, value2)}";

    
    protected static string GetQuery(string key1, string value1, string key2, string value2, string key3, string value3) =>
        $"?{GetQueryKeyValue(key1, value1)}&{GetQueryKeyValue(key2, value2)}&{GetQueryKeyValue(key3, value3)}";
    
    
    protected static string ToQueryString(Dictionary<string, string> queries)
    {
        StringBuilder builder = new("?");

        foreach (var query in queries)
        {
            builder.Append($"{HttpUtility.UrlEncode(query.Key)}={HttpUtility.UrlEncode(query.Value)}");
            builder.Append('&');
        }

        builder.Remove(builder.Length - 1, 1);
        
        return builder.ToString();
    }
    
    
    private static string GetQueryKeyValue(string key, string value) =>
        $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";
}