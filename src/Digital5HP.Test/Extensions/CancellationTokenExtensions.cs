namespace Digital5HP.Test
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    public static class CancellationTokenExtensions
    {
        public static IEnumerable<Action<object>> Registrations(this CancellationToken token)
        {
            var sourceFieldInfo =
                typeof(CancellationToken).GetField("_source", BindingFlags.NonPublic | BindingFlags.Instance);
            if (sourceFieldInfo == null)
            {
                throw new TestException(
                    $"Type '{nameof(CancellationToken)}' was modified. Cannot retrieve registrations.");
            }

            var cancellationTokenSource = (CancellationTokenSource) sourceFieldInfo.GetValue(token);

            var callbackPartitionsFieldInfo =
                typeof(CancellationTokenSource).GetField(
                    "_callbackPartitions",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            if (callbackPartitionsFieldInfo == null)
            {
                throw new TestException(
                    $"Type '{nameof(CancellationTokenSource)}' was modified. Cannot retrieve registrations.");
            }

            var callbackPartitions = (Array) callbackPartitionsFieldInfo.GetValue(cancellationTokenSource);

            if (callbackPartitions == null)
            {
                throw new TestException(
                    "CancellationTokenSource._callbackPartitions is null. Cannot retrieve registrations.");
            }

            foreach (var callbackPartition in callbackPartitions)
            {
                if (callbackPartition == null)
                {
                    continue;
                }

                var callbackPartitionType = callbackPartition.GetType();

                var callbacksFieldInfo = callbackPartitionType.GetField("Callbacks");
                if (callbacksFieldInfo == null)
                {
                    throw new TestException(
                        $"Type '{callbackPartitionType.Name}' was modified. Cannot retrieve registrations.");
                }

                var callbacks = callbacksFieldInfo.GetValue(callbackPartition);

                if (callbacks == null)
                {
                    throw new TestException(
                        "CancellationTokenSource._callbackPartitions.Callbacks is null. Cannot retrieve registrations.");
                }

                var callbackNodeType = callbacks.GetType();

                var callbackFieldInfo = callbackNodeType.GetField("Callback");
                if (callbackFieldInfo == null)
                {
                    throw new TestException(
                        $"Type '{callbackNodeType.Name}' was modified. Cannot retrieve registrations.");
                }

                var callback = (Action<object>) callbackFieldInfo.GetValue(callbacks);

                if (callback != null)
                {
                    yield return callback;
                }
            }
        }
    }
}