using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    internal class FetchClass: EvalAndWaitForEventBase
    {
        public FetchClass(AsyncFirefoxDriver browserClient) : base(browserClient)
        {
        }

        public async Task<FetchResult> Fetch(string url)
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



    }
}
