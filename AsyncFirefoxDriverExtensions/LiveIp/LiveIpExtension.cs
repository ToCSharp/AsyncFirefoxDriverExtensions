using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class LiveIpExtension
    {
        public static async Task<LiveIpResult> GetLiveIp(this IAsyncWebBrowserClient browserClient)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            await browserClient.AddSendEventFuncIfNo();
            browserClient.AddEventListener("EvalAndWaitForEventLiveIp", OnEvalAndWaitForEvent);
            var evalStrAddId = @"fetch('http://liveipaddress.intelcomms.net/show/')
    .then(response => response.text())
    .then(str => (new window.DOMParser()).parseFromString(str, 'text/xml'))
    .then(data => top.zuSendEvent({{ 'to': 'EvalAndWaitForEventLiveIp', 'id': {0}, 'res': data.getElementsByTagName('ip_address')[0].textContent }}))
    .catch(err => top.zuSendEvent({{ 'to': 'EvalAndWaitForEventLiveIp', 'id': {0}, 'error': err.toString() }}));
";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            var res = new LiveIpResult
            {
                Ip = resJson?["res"]?["value"]?.ToString(),
                Error = resJson?["error"]?["value"]?.ToString(),
            };
            return res;
        }


        private static int idEvalAndWaitForEvent = 1;
        //private static string EvalAndWaitForEventName { get; } = "EvalAndWaitForEventLiveIp";
        private static ConcurrentDictionary<int, TaskCompletionSource<JToken>> evalAndWaitForEventAsyncTasks = new ConcurrentDictionary<int, TaskCompletionSource<JToken>>();
        private static async Task<JToken> EvalAndWaitForEvent(IAsyncWebBrowserClient browserClient, string evalStrAddId, /*int id, */CancellationToken cancellationToken = new CancellationToken())
        {
            var id = idEvalAndWaitForEvent++;
            try
            {
                var evalStr = string.Format(evalStrAddId, id);
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
