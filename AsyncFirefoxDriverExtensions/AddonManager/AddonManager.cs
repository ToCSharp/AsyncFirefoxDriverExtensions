using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public class AddonManager
    {
        private IAsyncWebBrowserClient browserClient;

        public AddonManager(IAsyncWebBrowserClient browserClient)
        {
            this.browserClient = browserClient;
        }

        public async Task<List<AddonData>> GetAddonsList(CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            browserClient.AddEventListener("EvalAndWaitForEventAddonManager", OnEvalAndWaitForEvent);
            var evalStrAddId = @"
AddonManager.getAllAddons().then(a => {
    let addons = [];
    for (var i = 0; i < a.length; i++) {
        addons.push({
            'id': a[i].id,
            'version': a[i].version,
            'name': a[i].name,
            'description': a[i].description,
            'type': a[i].type,
            'isWebExtension': a[i].isWebExtension,
            'temporarilyInstalled': a[i].temporarilyInstalled,
            'hasEmbeddedWebExtension': a[i].hasEmbeddedWebExtension,
            'aboutURL': a[i].aboutURL,
            'optionsURL': a[i].optionsURL,
            'optionsType': a[i].optionsType,
            'iconURL': a[i].iconURL,
            'icon64URL': a[i].icon64URL,
            'applyBackgroundUpdates': a[i].applyBackgroundUpdates,
            'syncGUID': a[i].syncGUID,
            'scope': a[i].scope,
            'pendingOperations': a[i].pendingOperations,
            'operationsRequiringRestart': a[i].operationsRequiringRestart,
            'isDebuggable': a[i].isDebuggable,
            'permissions': a[i].permissions,
            'isActive': a[i].isActive,
            'userDisabled': a[i].userDisabled,
            'softDisabled': a[i].softDisabled,
            'isSystem': a[i].isSystem,
            'isSyncable': a[i].isSyncable,
            'userPermissions': a[i].userPermissions,
            'isCompatible': a[i].isCompatible,
            'isPlatformCompatible': a[i].isPlatformCompatible,
            'providesUpdatesSecurely': a[i].providesUpdatesSecurely,
            'blocklistState': a[i].blocklistState,
            'blocklistURL': a[i].blocklistURL,
            'appDisabled': a[i].appDisabled,
            'skinnable': a[i].skinnable,
            'size': a[i].size,
            'foreignInstall': a[i].foreignInstall,
            'hasBinaryComponents': a[i].hasBinaryComponents,
            'strictCompatibility': a[i].strictCompatibility,
            'updateURL': a[i].updateURL,
            'multiprocessCompatible': a[i].multiprocessCompatible,
            'signedState': a[i].signedState,
            'mpcOptedOut': a[i].mpcOptedOut,
            'isCorrectlySigned': a[i].isCorrectlySigned,
            'fullDescription': a[i].fullDescription,
            'developerComments': a[i].developerComments,
            'eula': a[i].eula,
            'supportURL': a[i].supportURL,
            'contributionURL': a[i].contributionURL,
            'contributionAmount': a[i].contributionAmount,
            'averageRating': a[i].averageRating,
            'reviewCount': a[i].reviewCount,
            'reviewURL': a[i].reviewURL,
            'totalDownloads': a[i].totalDownloads,
            'weeklyDownloads': a[i].weeklyDownloads,
            'dailyUsers': a[i].dailyUsers,
            'repositoryStatus': a[i].repositoryStatus,
            'sourceURI': a[i].sourceURI ? JSON.stringify(a[i].sourceURI) : '',
            'releaseNotesURI': a[i].releaseNotesURI,
            'creator': a[i].creator ? JSON.stringify(a[i].creator) : '',
            'homepageURL': a[i].homepageURL,
            'developers': a[i].developers ? JSON.stringify(a[i].contributors) : '',
            'translators': a[i].translators ? JSON.stringify(a[i].contributors) : '',
            'contributors': a[i].contributors ? JSON.stringify(a[i].contributors) : '',
        })
    }
        
    top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'addons': JSON.stringify(addons) });
}
);

";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            try
            {
                return JsonConvert.DeserializeObject<List<AddonData>>(resJson["addons"]?["value"].ToString());
            }
            catch(Exception ex)
            {
                return new List<AddonData> { new AddonData { Name = ex.ToString() } };
            }
        }

        public async Task<InstallAddonResult> InstallTemporaryAddon(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            browserClient.AddEventListener("EvalAndWaitForEventAddonManager", OnEvalAndWaitForEvent);
            var evalStrAddId = @"
    Cu.import('resource://gre/modules/FileUtils.jsm');
    let file = new FileUtils.File(" + path + @");
    let listener = {
      onInstallEnded: function (install, addon) {
        top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'addonId': addon.id });
      },

      onInstallFailed: function (install) {
        top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'error': install.error });
      },

      onInstalled: function (addon) {
        AddonManager.removeAddonListener(listener);
        top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'addonId': addon.id });
      }
    }
      AddonManager.addAddonListener(listener);
      AddonManager.installTemporaryAddon(file);

";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            var res = new InstallAddonResult
            {
                AddonId = resJson?["addonId"]?["value"]?.ToString(),
                Error = resJson?["error"]?["value"]?.ToString(),
            };
            return res;
        }

        public async Task<InstallAddonResult> InstallAddon(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            browserClient.AddEventListener("EvalAndWaitForEventAddonManager", OnEvalAndWaitForEvent);
            var evalStrAddId = @"
    Cu.import('resource://gre/modules/FileUtils.jsm');
    let file = new FileUtils.File('" + path.Replace("\\", "\\\\") + @"');
    let listener = {
      onInstallEnded: function (install, addon) {
        top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'addonId': addon.id });
      },

      onInstallFailed: function (install) {
        top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'error': install.error });
      },

      onInstalled: function (addon) {
        AddonManager.removeAddonListener(listener);
        top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'addonId': addon.id });
      }
    }
      AddonManager.getInstallForFile(file, function (aInstall) {
        if (aInstall.error !== 0) {
            top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_, 'error': install.error });
        }
        aInstall.addListener(listener);
        aInstall.install();
      });
";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            var res = new InstallAddonResult
            {
                AddonId = resJson?["addonId"]?["value"]?.ToString(),
                Error = resJson?["error"]?["value"]?.ToString(),
            };
            return res;
        }

        public async Task UninstallAddon(string addonId, CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            browserClient.AddEventListener("EvalAndWaitForEventAddonManager", OnEvalAndWaitForEvent);
            var evalStrAddId = @"
    AddonManager.getAddonByID('" + addonId + @"', function (addon) {
       addon.uninstall();
       top.zuSendEvent({ 'to': 'EvalAndWaitForEventAddonManager', 'id': _AddIdForEventHere_ });
    });    

";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
        }

        private static int idEvalAndWaitForEvent = 1;
        //private static string EvalAndWaitForEventName { get; } = "EvalAndWaitForEventLiveIp";
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
