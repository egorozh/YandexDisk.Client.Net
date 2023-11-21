using System;
using YandexDisk.Client.Clients;


namespace YandexDisk.Client;


/// <summary>
/// Definition of all methods od Yandex Disk API
/// </summary>
public interface IDiskApi : IDisposable
{
    /// <summary>
    /// Uploading and downloading file operation
    /// </summary>
    IFilesClient Files { get; }

    /// <summary>
    /// Getting files and folders metadata  
    /// </summary>
    IMetaInfoClient MetaInfo { get; }

    /// <summary>
    /// Manipulating with existing files and folders 
    /// </summary>
    ICommandsClient Commands { get; }
}