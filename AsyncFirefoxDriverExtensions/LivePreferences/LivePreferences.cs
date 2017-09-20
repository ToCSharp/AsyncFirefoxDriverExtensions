using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Zu.WebBrowser;

namespace Zu.Firefox
{
    public class LivePreferences
    {
        private AsyncFirefoxDriver browserClient;

        public LivePreferences(AsyncFirefoxDriver browserClient)
        {
            this.browserClient = browserClient;
        }

        public async Task<JToken> Set(string path, string value)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
            var res = await browserClient?.EvalInChrome($@"try {{
var {{ require }} = Cu.import('resource://devtools/shared/Loader.jsm', {{}});
var preferences = require('sdk/preferences/service');
preferences.set('{path}', {value});
}} catch(ex) {{
return ex.toString();
}}
return ""ok"";
");
            return res;
        }
        public async Task<JToken> SetLocalized(string path, string value)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
            var res = await browserClient?.EvalInChrome($@"try {{
var {{ require }} = Cu.import('resource://devtools/shared/Loader.jsm', {{}});
var preferences = require('sdk/preferences/service');
preferences.setLocalized('{path}', {value});
}} catch(ex) {{
return ex.toString();
}}
return ""ok"";
");
            return res;
        }
        public async Task<string> Get(string path)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
            var res = await browserClient?.EvalInChrome($@"try {{
var {{ require }} = Cu.import('resource://devtools/shared/Loader.jsm', {{}});
var preferences = require('sdk/preferences/service');
return preferences.get('{path}');
}} catch(ex) {{
return ex.toString();
}}
");
            return res?["value"].ToString();
        }
        public async Task<string> GetLocalized(string path)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
            var res = await browserClient?.EvalInChrome($@"try {{
var {{ require }} = Cu.import('resource://devtools/shared/Loader.jsm', {{}});
var preferences = require('sdk/preferences/service');
return preferences.getLocalized('{path}');
}} catch(ex) {{
return ex.toString();
}}
");
            return res?["value"].ToString();
        }
        public async Task<string> Reset(string path)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
            var res = await browserClient?.EvalInChrome($@"try {{
var {{ require }} = Cu.import('resource://devtools/shared/Loader.jsm', {{}});
var preferences = require('sdk/preferences/service');
preferences.reset('{path}');
}} catch(ex) {{
return ex.toString();
}}
return 'ok';
");
            return res?["value"].ToString();
        }

        public async Task<string> IsSet(string path)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
            var res = await browserClient?.EvalInChrome($@"try {{
var {{ require }} = Cu.import('resource://devtools/shared/Loader.jsm', {{}});
var preferences = require('sdk/preferences/service');
return preferences.isSet('{path}');
}} catch(ex) {{
return ex.toString();
}}
");
            return res?["value"].ToString();
        }
        public async Task<string> Has(string path)
        {
            if (browserClient == null) throw new ArgumentException(nameof(browserClient));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
            var res = await browserClient?.EvalInChrome($@"try {{
var {{ require }} = Cu.import('resource://devtools/shared/Loader.jsm', {{}});
var preferences = require('sdk/preferences/service');
return preferences.has('{path}');
}} catch(ex) {{
return ex.toString();
}}
");
            return res?["value"].ToString();
        }

    }
}
