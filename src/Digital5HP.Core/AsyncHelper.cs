namespace Digital5HP;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

// Taken from Microsoft.AspNet.Identity.Core (see https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/AsyncHelper.cs)
public static class AsyncHelper
{
    private static readonly TaskFactory TaskFactory = new(
        CancellationToken.None,
        TaskCreationOptions.None,
        TaskContinuationOptions.None,
        TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
#pragma warning disable CA2008
        return TaskFactory.StartNew(
                               () =>
                               {
                                   Thread.CurrentThread.CurrentCulture = culture;
                                   Thread.CurrentThread.CurrentUICulture = cultureUi;
                                   return func();
                               })
#pragma warning restore CA2008
                          .Unwrap()
                          .GetAwaiter()
#pragma warning disable VSTHRD002
                          .GetResult();
#pragma warning restore VSTHRD002
    }

    public static void RunSync(Func<Task> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
#pragma warning disable CA2008
        TaskFactory.StartNew(
                        () =>
                        {
                            Thread.CurrentThread.CurrentCulture = culture;
                            Thread.CurrentThread.CurrentUICulture = cultureUi;
                            return func();
                        })
#pragma warning restore CA2008
                   .Unwrap()
                   .GetAwaiter()
#pragma warning disable VSTHRD002
                   .GetResult();
#pragma warning restore VSTHRD002
    }
}
