using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class LiveIpExtension
    {
        public static Task<LiveIpResult> GetLiveIp(this WebDriver webDriver) => (webDriver?.browserClient as AsyncFirefoxDriver)?.GetLiveIp();

        public static Task<LiveIpResult> GetLiveIp(this AsyncFirefoxDriver browserClient)
            => new LiveIp(browserClient).GetLiveIp();

    }
}
