using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public class EvalAndWaitForEventBase
    {
        public IAsyncWebBrowserClient browserClient;

        public EvalAndWaitForEventBase(IAsyncWebBrowserClient browserClient)
        {
            this.browserClient = browserClient;
        }
        private int idEvalAndWaitForEvent = 0;
        private ConcurrentDictionary<int, TaskCompletionSource<JToken>> evalAndWaitForEventAsyncTasks = new ConcurrentDictionary<int, TaskCompletionSource<JToken>>();

        public async Task<JToken> EvalAndWaitForEvent(IAsyncWebBrowserClient browserClient, string evalStrAddId, /*int id, */CancellationToken cancellationToken = new CancellationToken())
        {
            var id = Interlocked.Increment(ref idEvalAndWaitForEvent);
            try
            {
                var evalStr = evalStrAddId.Replace("_AddIdForEventHere_", id.ToString()); //  string.Format(evalStrAddId, id);
                var promise = evalAndWaitForEventAsyncTasks.GetOrAdd(id, i => new TaskCompletionSource<JToken>());

                var res = await browserClient?.Eval(evalStr.Replace("\\", "\\\\"));
                if (res?["error"] != null)
                {
                    return res;
                }
                else
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    cancellationToken.Register(() => promise.TrySetCanceled(), false);

                    var response = await promise.Task.ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    return response;
                }

            }
            finally
            {
                evalAndWaitForEventAsyncTasks.TryRemove(id, out TaskCompletionSource<JToken> promise);
            }

        }
        public void OnEvalAndWaitForEvent(JToken message)
        {
            try
            {
                if (int.TryParse(message?["id"]?["value"]?.ToString(), out int messageId))
                {
                    if (evalAndWaitForEventAsyncTasks.TryGetValue(messageId, out TaskCompletionSource<JToken> promise))
                    {
                        promise.SetResult(message);

                    }
                    else
                    {
                        //Debug.Fail(string.Format(CultureInfo.CurrentCulture, "Invalid response identifier '{0}'", messageId));
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}