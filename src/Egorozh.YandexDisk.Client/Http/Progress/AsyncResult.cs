﻿using System;
using System.Diagnostics.Contracts;
using System.Threading;


namespace Egorozh.YandexDisk.Client.Http.Progress;


internal abstract class AsyncResult(AsyncCallback callback, object state) : IAsyncResult
{
    private bool _isCompleted;
    private bool _completedSynchronously;
    private bool _endCalled;

    private Exception _exception;

    public object AsyncState => state;

    public WaitHandle AsyncWaitHandle
    {
        get
        {
            Contract.Assert(false, "AsyncWaitHandle is not supported -- use callbacks instead.");
            return null;
        }
    }

    public bool CompletedSynchronously
    {
        get { return _completedSynchronously; }
    }

    public bool HasCallback
    {
        get { return callback != null; }
    }

    public bool IsCompleted
    {
        get { return _isCompleted; }
    }

    protected void Complete(bool completedSynchronously)
    {
        if (_isCompleted)
        {
            //throw JSType.Error.InvalidOperation(Properties.Resources.AsyncResult_MultipleCompletes, GetType().Name);
            throw new Exception("AsyncResult_MultipleCompletes");
        }

        _completedSynchronously = completedSynchronously;
        _isCompleted = true;

        if (callback != null)
        {
            try
            {
                callback(this);
            }
            catch (Exception e)
            {
                //throw JSType.Error.InvalidOperation(e, Properties.Resources.AsyncResult_CallbackThrewException);
                throw new Exception("AsyncResult_CallbackThrewException");
            }
        }
    }

    protected void Complete(bool completedSynchronously, Exception exception)
    {
        _exception = exception;
        Complete(completedSynchronously);
    }

    protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult : AsyncResult
    {
        if (result == null)
        {
            ArgumentNullException.ThrowIfNull(result);
        }

        TAsyncResult thisPtr = result as TAsyncResult;

        if (thisPtr == null)
        {
            ArgumentNullException.ThrowIfNull(thisPtr);
        }

        if (!thisPtr._isCompleted)
        {
            thisPtr.AsyncWaitHandle.WaitOne();
        }

        if (thisPtr._endCalled)
        {
            //throw JSType.Error.InvalidOperation(Properties.Resources.AsyncResult_MultipleEnds);
            throw new Exception("AsyncResult_MultipleEnds");
        }

        thisPtr._endCalled = true;

        if (thisPtr._exception != null)
        {
            throw thisPtr._exception;
        }

        return thisPtr;
    }
}