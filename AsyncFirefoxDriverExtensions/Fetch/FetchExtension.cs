using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class FetchExt
    {
        public static Task<FetchResult> Fetch(this WebDriver webDriver, string url) => webDriver?.browserClient?.Fetch(url);

        public static async Task<FetchResult> Fetch(this IAsyncWebBrowserClient browserClient, string url)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            await browserClient.AddSendEventFuncIfNo();
            browserClient.AddEventListener("EvalAndWaitForEventFetch", OnEvalAndWaitForEvent);
            var evalStrAddId = @"fetch('" + url + @"')
    .then(response => response.text())
    .then(str => top.zuSendEvent({ 'to': 'EvalAndWaitForEventFetch', 'id': _AddIdForEventHere_, 'res': str }))
    .catch(err => top.zuSendEvent({ 'to': 'EvalAndWaitForEventFetch', 'id': _AddIdForEventHere_, 'error': err.toString() }));
";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            var res = new FetchResult
            {
                Result = resJson?["res"]?["value"]?.ToString(),
                Error = (resJson?["error"] as JValue)?.ToString() ?? resJson?["error"]?["value"]?.ToString(),
            };
            return res;
        }


        private static int idEvalAndWaitForEvent = 1;
        private static ConcurrentDictionary<int, TaskCompletionSource<JToken>> evalAndWaitForEventAsyncTasks = new ConcurrentDictionary<int, TaskCompletionSource<JToken>>();
        private static async Task<JToken> EvalAndWaitForEvent(IAsyncWebBrowserClient browserClient, string evalStrAddId, /*int id, */CancellationToken cancellationToken = new CancellationToken())
        {
            var id = idEvalAndWaitForEvent++;
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
        private static void OnEvalAndWaitForEvent(JToken message)
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
