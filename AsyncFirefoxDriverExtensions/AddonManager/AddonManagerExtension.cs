﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zu.AsyncWebDriver.Remote;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public static class AddonManagerExtension
    {
        public static AddonManager AddonManager(this AsyncFirefoxDriver browserClient)
        {
            return new AddonManager(browserClient);
        }

        public static AddonManager AddonManager(this WebDriver webDriver) => (webDriver?.browserClient as AsyncFirefoxDriver)?.AddonManager();

    }
}
