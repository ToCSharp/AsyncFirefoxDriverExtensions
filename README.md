# AsyncFirefoxDriverExtensions

Extensions for [AsyncFirefoxDriver](https://github.com/ToCSharp/AsyncWebDriver).

* LiveIp to get ip
* LivePreferences to view and edit Firefox preferences of running profile
* AddonManager have methods GetAddonsList, InstallAddon, InstallTemporaryAddon, UninstallAddon
* CacheStorage with GetCacheInfo, GetEntryHeaders, SaveEntryDataToFile, GetEntryData and Clear methods
* Fetch(url) to load any file
* You say, what to do first. On gitter or in issues.

[![Join the chat at https://gitter.im/AsyncWebDriver/Lobby](https://badges.gitter.im/AsyncWebDriver/Lobby.svg)](https://gitter.im/AsyncWebDriver/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Usage
### Install AsyncFirefoxDriverExtensions via NuGet

If you want to include AsyncFirefoxDriverExtensions in your project, you can [install it directly from NuGet](https://www.nuget.org/packages/AsyncFirefoxDriverExtensions/)
```
PM> Install-Package AsyncFirefoxDriverExtensions
```
### Write code example
```csharp
            var ip = await firefoxDriver.GetLiveIp();
             
            var prefPath = "intl.accept_languages";
            var res = await firefoxDriver.LivePreferences().GetLocalized(prefPath);
            await firefoxDriver.LivePreferences().Set(prefPath, "'en-us,en'");
            
            var addons = await firefoxDriver.AddonManager().GetAddonsList();
            var res = await firefoxDriver.AddonManager().InstallAddon(addonPath);
            var res = await firefoxDriver.AddonManager().InstallTemporaryAddon(addonPath2);
            await firefoxDriver.AddonManager().UninstallAddon(addonId);
            
            var cacheInfo = await firefoxDriver.CacheStorage().GetCacheInfo();

```

## Examples
Look at FirefoxDriverExtensionsExample.

Run built Example in release tab.
