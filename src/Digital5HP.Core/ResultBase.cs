namespace Digital5HP;

using System;

public abstract class ResultBase<T>
    where T : ResultBase<T>, new()
{
    protected ResultBase()
    {
        this.IsSuccessful = true;
    }

    public bool IsSuccessful { get; protected set; }

    public string Message { get; protected set; }

    public Exception Exception { get; protected set; }

#pragma warning disable CA1000, MA0018
    public static T Failed(string message, Exception exception = null)
#pragma warning restore CA1000, MA0018
    {
        var result = Activator.CreateInstance<T>();
        result.IsSuccessful = false;
        result.Message = message;
        result.Exception = exception;

        return result;
    }

    public TOther ConvertTo<TOther>()
        where TOther : ResultBase<TOther>, new()
    {
        var result = Activator.CreateInstance<TOther>();
        result.IsSuccessful = this.IsSuccessful;
        result.Message = this.Message;
        result.Exception = this.Exception;

        return result;
    }
}
