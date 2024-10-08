﻿using System.ComponentModel;

namespace Egorozh.YandexDisk.Client.Http.Progress;

public class HttpProgressEventArgs : ProgressChangedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpProgressEventArgs"/> with the parameters given.
    /// </summary>
    /// <param name="progressPercentage">The percent completed of the overall exchange.</param>
    /// <param name="userToken">Any user state provided as part of reading or writing the data.</param>
    /// <param name="bytesTransferred">The current number of bytes either received or sent.</param>
    /// <param name="totalBytes">The total number of bytes expected to be received or sent.</param>
    public HttpProgressEventArgs(int progressPercentage, object userToken, long bytesTransferred, long? totalBytes)
        : base(progressPercentage, userToken)
    {
        BytesTransferred = bytesTransferred;
        TotalBytes = totalBytes;
    }

    /// <summary>
    /// Gets the current number of bytes transferred.
    /// </summary>
    public long BytesTransferred { get; private set; }

    /// <summary>
    /// Gets the total number of expected bytes to be sent or received. If the number is not known then this is null.
    /// </summary>
    public long? TotalBytes { get; private set; }
}