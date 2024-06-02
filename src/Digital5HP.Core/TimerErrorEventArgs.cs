namespace Digital5HP;

using System;

public class TimerErrorEventArgs(Exception exception) : EventArgs
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Exception Exception { get; } = exception;
}
