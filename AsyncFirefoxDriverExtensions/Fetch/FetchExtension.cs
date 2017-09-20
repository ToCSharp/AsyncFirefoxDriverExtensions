using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class FetchExtension
    {
 
        public static Task<FetchResult> Fetch(this WebDriver webDriver, string url) => (webDriver?.browserClient as AsyncFirefoxDriver)?.Fetch(url);

        public static Task<FetchResult> Fetch(this AsyncFirefoxDriver browserClient, string url)
            => new FetchClass(browserClient).Fetch(url);

    }
}
