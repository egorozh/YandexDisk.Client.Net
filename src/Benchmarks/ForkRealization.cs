using System.Net;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace Benchmarks;

public static class ForkRealization
{
    private static readonly TestHttpClient _httpClientTest = new(
        httpStatusCode: HttpStatusCode.OK,
        result: """

                {
                  "public_key": "HQsmHLoeyBlJf8Eu1jlmzuU+ZaLkjPkgcvmokRUCIo8=",
                  "_embedded": {
                    "sort": "",
                    "path": "disk:/foo",
                    "items": [
                      {
                        "path": "disk:/foo/bar",
                        "type": "dir",
                        "name": "bar",
                        "modified": "2014-04-22T10:32:49+04:00",
                        "created": "2014-04-22T10:32:49+04:00"
                      },
                      {
                        "name": "photo.png",
                        "preview": "https://downloader.disk.yandex.ru/preview/...",
                        "created": "2014-04-21T14:57:13+04:00",
                        "modified": "2014-04-21T14:57:14+04:00",
                        "path": "disk:/foo/photo.png",
                        "md5": "4334dc6379c8f95ddf11b9508cfea271",
                        "type": "file",
                        "mime_type": "image/png",
                        "size": 34567
                      }
                    ],
                    "limit": 20,
                    "offset": 0
                  },
                  "name": "foo",
                  "created": "2014-04-21T14:54:42+04:00",
                  "custom_properties": {"foo":"1", "bar":"2"},
                  "public_url": "https://yadi.sk/d/2AEJCiNTZGiYX",
                  "modified": "2014-04-22T10:32:49+04:00",
                  "path": "disk:/foo",
                  "type": "dir"
                }

                """);
    
    public static async Task GetInfoAsync()
    { 
        var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl, 
            TestHttpClient.ApiKey, 
            logSaver: null,
            httpClient: _httpClientTest);

        Resource result = await diskClient.MetaInfo.GetInfoAsync(new ResourceRequest
        {
            Path = "/",
            Limit = 20,
            Offset = 0,
            Sort = "name"
        }, CancellationToken.None).ConfigureAwait(false);
    }
}