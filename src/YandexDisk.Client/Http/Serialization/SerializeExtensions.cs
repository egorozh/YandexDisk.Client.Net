using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using YandexDisk.Client.Protocol;


namespace YandexDisk.Client.Http.Serialization;


internal static class SerializeExtensions
{
    public static T DeserializeResponse<T>(this HttpObject response, IJsonTypeInfoResolver typeInfo) where T : new()
    {
        if (response.CheckIsNull())
            throw new Exception($"{nameof(SerializeExtensions)}.{nameof(DeserializeResponse)} - response is null");

        string json = response.ToJsonContent();

        try
        {
            var result = JsonSerializer.Deserialize<T>(json, CreateOptions(typeInfo))
                   ?? throw new Exception($"{nameof(SerializeExtensions)}.{nameof(DeserializeResponse)} - response is null");;
            
            //If response body is null but ProtocolObjectResponse was requested, 
            //create empty object
            
            if (result == null && typeof(ProtocolObjectResponse).IsAssignableFrom(typeof(T)))
            {
                result = new T();
            }

            //If response is ProtocolObjectResponse,
            //add HttpStatusCode to response
            if (result is ProtocolObjectResponse protocolObject)
            {
                protocolObject.HttpStatusCode = response.StatusCode;
            }

            return result;
        }
        catch (Exception e)
        {
            throw new Exception($"{nameof(SerializeExtensions)}.{nameof(DeserializeResponse)} - {e}");
        }
    }


    private static JsonSerializerOptions CreateOptions(IJsonTypeInfoResolver typeInfo)
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = typeInfo
        };

        // private readonly MediaTypeFormatter[] _defaultFormatters =
// {
//     new JsonMediaTypeFormatter
//     {
//         SerializerSettings =
//         {
//           
//             DateFormatHandling = DateFormatHandling.IsoDateFormat,
//             DateTimeZoneHandling = DateTimeZoneHandling.Utc,
//             DateParseHandling = DateParseHandling.DateTime
//         }
//     }
// };

        return options;
    }
}