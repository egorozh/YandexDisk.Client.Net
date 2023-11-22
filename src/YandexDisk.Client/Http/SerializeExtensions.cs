using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using YandexDisk.Client.Protocol;


namespace YandexDisk.Client.Http;


internal static class SerializeExtensions
{
    public static T DeserializeResponse<T>(this HttpObject response, JsonTypeInfo<T> typeInfo) where T : new()
    {
        if (response.CheckIsNull())
            throw new Exception($"{nameof(SerializeExtensions)}.{nameof(DeserializeResponse)} - response is null");

        string json = response.ToJsonContent();

        try
        {
            var result = JsonSerializer.Deserialize(json, typeInfo)
                   ?? throw new Exception($"{nameof(SerializeExtensions)}.{nameof(DeserializeResponse)} - response is null");;
            
            //If response body is null but ProtocolObjectResponse was requested, 
            //create empty object
            
            if (typeof(ProtocolObjectResponse).IsAssignableFrom(typeof(T)))
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
}