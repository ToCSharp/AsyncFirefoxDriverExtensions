using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class LivePreferencesExtension
    {
        public static LivePreferences LivePreferences(this IAsyncWebBrowserClient browserClient)
        {
            return new LivePreferences(browserClient);
        }

        public static LivePreferences LivePreferences(this WebDriver webDriver) => webDriver?.LivePreferences();

    }
}
