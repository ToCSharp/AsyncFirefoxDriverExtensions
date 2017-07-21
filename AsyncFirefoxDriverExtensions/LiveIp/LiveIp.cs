using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public class LiveIp: EvalAndWaitForEventBase
    {
        public LiveIp(IAsyncWebBrowserClient browserClient) : base(browserClient)
        {
        }

        public async Task<LiveIpResult> GetLiveIp()
        {
            if (browserClient == null) throw new MemberAccessException(nameof(browserClient));
            await browserClient.AddSendEventFuncIfNo();
            browserClient.AddEventListener("EvalAndWaitForEventLiveIp", OnEvalAndWaitForEvent);
            var evalStrAddId = @"fetch('http://liveipaddress.intelcomms.net/show/')
    .then(response => response.text())
    .then(str => (new window.DOMParser()).parseFromString(str, 'text/xml'))
    .then(data => top.zuSendEvent({ 'to': 'EvalAndWaitForEventLiveIp', 'id': _AddIdForEventHere_, 'res': data.getElementsByTagName('ip_address')[0].textContent }))
    .catch(err => top.zuSendEvent({ 'to': 'EvalAndWaitForEventLiveIp', 'id': _AddIdForEventHere_, 'error': err.toString() }));
";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            var res = new LiveIpResult
            {
                Ip = resJson?["res"]?["value"]?.ToString(),
                Error = (resJson?["error"] as JValue)?.ToString() ?? resJson?["error"]?["value"]?.ToString(),
            };
            return res;
        }



    }
}
