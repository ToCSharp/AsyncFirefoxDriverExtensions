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
    public class CacheStorage : EvalAndWaitForEventBase
    {
        public CacheStorage(IAsyncWebBrowserClient browserClient) : base(browserClient)
        {
        }

        public async Task Clear(CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.SetContextChrome();
            await browserClient.Eval("Services.cache2.clear()");
        }

        public async Task<CacheInfo> GetCacheInfo(CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            await browserClient.SetContextChrome();
            browserClient.AddEventListener("EvalAndWaitForEventCacheStorage", OnEvalAndWaitForEvent);
            var evalStrAddId = @"
var cacheStorageInfo = {};
cacheStorageInfo.EntryCount = null;
cacheStorageInfo.Consumption = null;
cacheStorageInfo.Capacity = null;
cacheStorageInfo.DiskDirectory = null;
cacheStorageInfo.entries = [];
Services.cache2.diskCacheStorage(Services.loadContextInfo.default, false).asyncVisitStorage({
    onCacheStorageInfo: function (aEntryCount, aConsumption, aCapacity, aDiskDirectory) {
        cacheStorageInfo.EntryCount = aEntryCount;
        cacheStorageInfo.Consumption = aConsumption;
        cacheStorageInfo.Capacity = aCapacity;
        cacheStorageInfo.DiskDirectory = aDiskDirectory.path;
    },
    onCacheEntryInfo: function (aURI, aIdEnhance, aDataSize, aFetchCount, aLastModifiedTime, aExpirationTime, aPinned, aInfo) {
        cacheStorageInfo.entries.push({
            //'uri': aURI,
            'Url': aURI.spec,
            'IdEnhance': aIdEnhance,
            'DataSize': aDataSize,
            'FetchCount': aFetchCount,
            'LastModifiedTime': (new Date(aLastModifiedTime * 1000)).toLocaleString(), 
            'ExpirationTime': (new Date(aExpirationTime * 1000)).toLocaleString(),
            'Pinned': aPinned,
            //'Info': aInfo,
        })
    },
    onCacheEntryVisitCompleted: function () {
        top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'cacheStorageInfo': JSON.stringify(cacheStorageInfo) });
    }
}, true);

";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            try
            {
                return JsonConvert.DeserializeObject<CacheInfo>(resJson["cacheStorageInfo"]?["value"].ToString());
            }
            catch (Exception ex)
            {
                return new CacheInfo { Error = ex.ToString() };
            }
        }

        public Task<string> GetEntryHeaders(CacheEntry entry, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetEntryHeaders(entry.Url, cancellationToken);
        }

        public async Task<string> GetEntryHeaders(string url, CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            await browserClient.SetContextChrome();
            browserClient.AddEventListener("EvalAndWaitForEventCacheStorage", OnEvalAndWaitForEvent);
            var evalStrAddId = @"
var uri = Services.io.newURI('" + url + @"', null, null);
Services.cache2.diskCacheStorage(Services.loadContextInfo.default, false).asyncOpenURI(uri, '', Ci.nsICacheStorage.OPEN_READONLY, {
        onCacheEntryCheck: function (aEntry, aAppCache) {
            return Ci.nsICacheEntryOpenCallback.ENTRY_WANTED;
        },
        onCacheEntryAvailable: function (aEntry, aNew, aAppCache, aResult) {
            top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'headers': aEntry.getMetaDataElement('response-head') });
        }
    });
";
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            return resJson["headers"]?["value"]?.ToString();
        }

        public Task<SaveEntryResult> SaveEntryDataToFile(CacheEntry entry, string filePath, CancellationToken cancellationToken = new CancellationToken())
        {
            return SaveEntryDataToFile(entry.Url, filePath, cancellationToken);
        }

        public async Task<SaveEntryResult> SaveEntryDataToFile(string url, string filePath, CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            await browserClient.SetContextChrome();
            browserClient.AddEventListener("EvalAndWaitForEventCacheStorage", OnEvalAndWaitForEvent);

            var scr = @"
_saveCacheEntry = function(aUrl, aFilePath) {
    return new Promise((resolve, reject) => {
        var uri = Services.io.newURI(aUrl, null, null);
        Services.cache2.diskCacheStorage(Services.loadContextInfo.default, false).asyncOpenURI(uri, '', Ci.nsICacheStorage.OPEN_READONLY, {
            onCacheEntryCheck: function (aEntry, aAppCache) {
                return Ci.nsICacheEntryOpenCallback.ENTRY_WANTED;
            },
            onCacheEntryAvailable: function (aEntry, aNew, aAppCache, aResult) {
                try {
                    var encode = null;
                    if (aEntry.getMetaDataElement('response-head').match(/Content-Encoding: (.+)$/m)) {
                        encode = RegExp.$1;
                    }
                    var BinaryInputStream = Components.Constructor('@mozilla.org/binaryinputstream;1', 'nsIBinaryInputStream', 'setInputStream');
                    var converterService = Cc['@mozilla.org/streamConverters;1'].getService(Ci.nsIStreamConverterService);
                    var readData = function (aEntry2, aEncode) {
                        return new Promise((resolve, reject) => {
                            let data = [];
                            let listener = {
                                onStartRequest: function (aRequest, aContext) { },
                                onDataAvailable: function (aRequest, aContext, aInputStream, aOffset, aCount) {
                                    data.push(new BinaryInputStream(aInputStream).readBytes(aCount));
                                },
                                onStopRequest: function (aRequest, aContext, aStatusCode) {
                                    resolve(data.join(''));
                                }
                            };
                            let inputStream = aEntry2.openInputStream(0);
                            let pump = Cc['@mozilla.org/network/input-stream-pump;1'].createInstance(Ci.nsIInputStreamPump);
                            pump.init(inputStream, 0, -1, 0, 0, true);
                            if (aEncode) {
                                listener = converterService.asyncConvertData(RegExp.$1, 'uncompressed', listener, null);
                            }
                            pump.asyncRead(listener, null);
                        });
                    }
                    readData(aEntry, encode).then(data => {
                        var fileStream = FileUtils.openFileOutputStream(new FileUtils.File(aFilePath),
                            FileUtils.MODE_WRONLY | FileUtils.MODE_CREATE | FileUtils.MODE_TRUNCATE);

                        fileStream.write(data, data.length);
                        if (fileStream instanceof Ci.nsISafeOutputStream) {
                            fileStream.finish();
                        } else {
                            fileStream.close();
                        }
                        resolve('saved');
                    });
                } catch (ex) { reject(ex.toString()); }
            }
        });
    });
} 
";
            await browserClient.ExecuteScript(scr, "saveCacheEntry.js");
            var evalStrAddId = @" 
    _saveCacheEntry('" + url + @"', '" + filePath.Replace("\\", "\\\\") + @"')
        .then(res => {
            top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'res': res });
        })
        .catch(err => top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'error': err.toString() }));
";

            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            var res = new SaveEntryResult
            {
                Result = resJson?["res"]?["value"]?.ToString(),
                Error = (resJson?["error"] as JValue)?.ToString() ?? resJson?["error"]?["value"]?.ToString(),
            };
            return res;
        }


        public Task<SaveEntryResult> GetEntryData(CacheEntry entry, bool doTryConvertToUnicode = false, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetEntryData(entry.Url, doTryConvertToUnicode, cancellationToken);
        }

        // todo encoding problem
        public async Task<SaveEntryResult> GetEntryData(string url, bool doTryConvertToUnicode = false, CancellationToken cancellationToken = new CancellationToken())
        {
            await browserClient.AddSendEventFuncIfNo();
            await browserClient.SetContextChrome();
            browserClient.AddEventListener("EvalAndWaitForEventCacheStorage", OnEvalAndWaitForEvent);

            var scr = @"
_getCacheEntryData = function(aUrl) {
    return new Promise((resolve, reject) => {
        var uri = Services.io.newURI(aUrl, null, null);
        Services.cache2.diskCacheStorage(Services.loadContextInfo.default, false).asyncOpenURI(uri, '', Ci.nsICacheStorage.OPEN_READONLY, {
            onCacheEntryCheck: function (aEntry, aAppCache) {
                return Ci.nsICacheEntryOpenCallback.ENTRY_WANTED;
            },
            onCacheEntryAvailable: function (aEntry, aNew, aAppCache, aResult) {
                var encode = null;
                if (aEntry.getMetaDataElement('response-head').match(/Content-Encoding: (.+)$/m)) {
                    encode = RegExp.$1;
                }
                var BinaryInputStream = Components.Constructor('@mozilla.org/binaryinputstream;1', 'nsIBinaryInputStream', 'setInputStream');
                var converterService = Cc['@mozilla.org/streamConverters;1'].getService(Ci.nsIStreamConverterService);
                var readData = function (aEntry2, aEncode) {
                    return new Promise((resolve, reject) => {
                        let data = [];
                        let listener = {
                            onStartRequest: function (aRequest, aContext) { },
                            onDataAvailable: function (aRequest, aContext, aInputStream, aOffset, aCount) {
                                data.push(new BinaryInputStream(aInputStream).readBytes(aCount));
                            },
                            onStopRequest: function (aRequest, aContext, aStatusCode) {
                                resolve(data.join(''));
                            }
                        };
                        let inputStream = aEntry2.openInputStream(0);
                        let pump = Cc['@mozilla.org/network/input-stream-pump;1'].createInstance(Ci.nsIInputStreamPump);
                        pump.init(inputStream, 0, -1, 0, 0, true);
                        if (aEncode) {
                            listener = converterService.asyncConvertData(RegExp.$1, 'uncompressed', listener, null);
                        }
                        pump.asyncRead(listener, null);
                    });
                }
                readData(aEntry, encode).then(data => {
                    resolve(data);
                });
            }
        });
    });
} 
";
            await browserClient.ExecuteScript(scr);
            var evalStrAddId = "";
            if (doTryConvertToUnicode)
            {
                evalStrAddId = @" 
    _getCacheEntryData('" + url + @"')
        .then(res => {
                let res2 = res;
                try {
                    var converter = Cc['@mozilla.org/intl/scriptableunicodeconverter']
                        .createInstance(Ci.nsIScriptableUnicodeConverter);
                    converter.charset = 'UTF-8';
                    res2 = converter.ConvertToUnicode(res2);
                } catch (e) { }

                top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'res': res2 });
        })
        .catch(err => top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'error': err.toString() }));
";

            }
            else
            {
                evalStrAddId = @" 
    _getCacheEntryData('" + url + @"')
        .then(res => {
            top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'res': res });
        })
        .catch(err => top.zuSendEvent({ 'to': 'EvalAndWaitForEventCacheStorage', 'id': _AddIdForEventHere_, 'error': err.toString() }));
";
            }
            var resJson = await EvalAndWaitForEvent(browserClient, evalStrAddId, cancellationToken);
            browserClient.RemoveEventListener(OnEvalAndWaitForEvent);
            var res = new SaveEntryResult
            {
                Result = resJson?["res"]?["value"]?.ToString(),
                Error = (resJson?["error"] as JValue)?.ToString() ?? resJson?["error"]?["value"]?.ToString(),
            };
            return res;
        }


    }
}
