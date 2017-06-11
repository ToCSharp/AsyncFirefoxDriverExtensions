using Newtonsoft.Json;
using System.Text;

namespace Zu.Firefox
{
    public class AddonData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string version { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string isWebExtension { get; set; }
        public string temporarilyInstalled { get; set; }
        public string hasEmbeddedWebExtension { get; set; }
        public string aboutURL { get; set; }
        public string optionsURL { get; set; }
        public string optionsType { get; set; }
        public string iconURL { get; set; }
        public string icon64URL { get; set; }
        public string applyBackgroundUpdates { get; set; }
        public string syncGUID { get; set; }
        public string scope { get; set; }
        public string pendingOperations { get; set; }
        public string operationsRequiringRestart { get; set; }
        public string isDebuggable { get; set; }
        public string permissions { get; set; }
        public string isActive { get; set; }
        public string userDisabled { get; set; }
        public string softDisabled { get; set; }
        public string isSystem { get; set; }
        public string isSyncable { get; set; }
        public string userPermissions { get; set; }
        public string isCompatible { get; set; }
        public string isPlatformCompatible { get; set; }
        public string providesUpdatesSecurely { get; set; }
        public string blocklistState { get; set; }
        public string blocklistURL { get; set; }
        public string appDisabled { get; set; }
        public string skinnable { get; set; }
        public string size { get; set; }
        public string foreignInstall { get; set; }
        public string hasBinaryComponents { get; set; }
        public string strictCompatibility { get; set; }
        public string updateURL { get; set; }
        public string multiprocessCompatible { get; set; }
        public string signedState { get; set; }
        public string mpcOptedOut { get; set; }
        public string isCorrectlySigned { get; set; }
        public string fullDescription { get; set; }
        public string developerComments { get; set; }
        public string eula { get; set; }
        public string supportURL { get; set; }
        public string contributionURL { get; set; }
        public string contributionAmount { get; set; }
        public string averageRating { get; set; }
        public string reviewCount { get; set; }
        public string reviewURL { get; set; }
        public string totalDownloads { get; set; }
        public string weeklyDownloads { get; set; }
        public string dailyUsers { get; set; }
        public string repositoryStatus { get; set; }
        public string sourceURI { get; set; }
        public string releaseNotesURI { get; set; }
        public string creator { get; set; }
        public string homepageURL { get; set; }
        public string developers { get; set; }
        public string translators { get; set; }
        public string contributors { get; set; }

        public string GetInfo()
        {
            var sb = new StringBuilder();
            foreach (var prop in GetType().GetProperties())
            {
                var pv = prop.GetValue(this);
                if (pv != null)
                {
                    sb.AppendLine($"{prop.Name} = {pv}");
                }
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}