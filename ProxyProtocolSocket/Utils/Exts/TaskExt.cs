namespace ProxyProtocolSocket.Utils.Exts
{
    public static class TaskExt
    {
        public static async Task WaitWhileAsync(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            if (timeout >= 0)
            {
                using (var cts = new CancellationTokenSource())
                {
                    var tk = cts.Token;
                    var waitTask = Task.Run(async () =>
                    {
                        while (!tk.IsCancellationRequested && condition()) await Task.Delay(frequency, tk).ContinueWith(tsk => { });
                    }, tk);

                    if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                    {
                        cts.Cancel();
                        throw new TimeoutException();
                    }
                }
                
            }
            else
                while (condition()) await Task.Delay(frequency);
        }

        public static async Task WaitUntilAsync(Func<bool> condition, int frequency = 25, int timeout = -1) =>
            await WaitWhileAsync(() => !condition(), frequency, timeout);
    }
}
