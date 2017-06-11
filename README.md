# AsyncFirefoxDriverExtensions

Extensions for [AsyncFirefoxDriver](https://github.com/ToCSharp/AsyncWebDriver).

Now we have:
* LiveIp to get ip
* LivePreferences to view and edit Firefox preferences of running profile
* AddonManager have methods GetAddonsList, InstallAddon, InstallTemporaryAddon, UninstallAddon.

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
            
            var addons = await ffDriver.AddonManager().GetAddonsList();
            var res = await ffDriver.AddonManager().InstallAddon(addonPath);
            var res = await ffDriver.AddonManager().InstallTemporaryAddon(addonPath2);
            await ffDriver.AddonManager().UninstallAddon(addonId);
```

## Examples
Look at FirefoxDriverExtensionsExample.

Run built Example in release tab.
