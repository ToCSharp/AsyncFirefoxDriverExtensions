using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class AddonManagerExtension
    {
        public static AddonManager AddonManager(this IAsyncWebBrowserClient browserClient)
        {
            return new AddonManager(browserClient);
        }
    }
}
