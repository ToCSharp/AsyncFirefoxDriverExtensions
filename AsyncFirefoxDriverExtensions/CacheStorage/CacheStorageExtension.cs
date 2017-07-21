using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class CacheStorageExtension
    {
        public static CacheStorage CacheStorage(this IAsyncWebBrowserClient browserClient)
        {
            return new CacheStorage(browserClient);
        }

        public static CacheStorage CacheStorage(this WebDriver webDriver) => webDriver?.browserClient?.CacheStorage();

    }
}
