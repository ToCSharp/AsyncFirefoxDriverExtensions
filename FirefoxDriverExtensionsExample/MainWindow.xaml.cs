using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Zu.Firefox;

namespace FirefoxDriverExtensionsExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AsyncFirefoxDriver ffDriver;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var profileName = tbProfileName.Text;
            ffDriver = new AsyncFirefoxDriver(profileName);
            await ffDriver.Connect();
            tblOpened.Text = "opened";
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var prefPath = tbPrefPath.Text;
            var res = await ffDriver.LivePreferences().Get(prefPath);
            tbGetPrefRes.Text = res;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var prefPath = tbPrefPath.Text;
            var newValue = tbPrefNewValue.Text;
            var res = await ffDriver.LivePreferences().Set(prefPath, newValue);
            tblRes.Text = res.ToString();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var prefPath = tbPrefPath.Text;
            var res = await ffDriver.LivePreferences().GetLocalized(prefPath);
            tbGetLocalizedPrefRes.Text = res;
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var prefPath = tbPrefPath.Text;
            var newValue = tbPrefNewValue.Text;
            var res = await ffDriver.LivePreferences().SetLocalized(prefPath, newValue);
            tblRes.Text = res.ToString();
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var prefPath = tbPrefPath.Text;
            var res = await ffDriver.LivePreferences().Reset(prefPath);
            tblRes.Text = res;
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var prefPath = tbPrefPath.Text;
            var res = await ffDriver.LivePreferences().IsSet(prefPath);
            tblRes.Text = res;
        }

        private async void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var prefPath = tbPrefPath.Text;
            var res = await ffDriver.LivePreferences().Has(prefPath);
            tblRes.Text = res;
        }

        private async void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var res = await ffDriver.GetLiveIp();
            tbIpRes.Text = res.Ip ?? res.Error;
        }

        private async void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var res = await ffDriver.AddonManager().GetAddonsList();
            lbAddons.ItemsSource = res;
        }

        private void lbAddons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ad = lbAddons.SelectedItem as AddonData;
            if(ad != null) tbAddonData.Text = ad.GetInfo();
        }

        private async void Button_Click_10(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var res = await ffDriver.AddonManager().InstallAddon(tbAddonPath.Text);
            tbAddonData.Text = res.AddonId ?? res.Error;
        }

        private async void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var ad = lbAddons.SelectedItem as AddonData;
            if (!string.IsNullOrWhiteSpace(ad?.Id))
            {
                await ffDriver.AddonManager().UninstallAddon(ad.Id);
                tbAddonData.Text = ad?.Id + " uninstalled.";
                var res = await ffDriver.AddonManager().GetAddonsList();
                lbAddons.ItemsSource = res;
            }

        }

        private async void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var res = await ffDriver.AddonManager().InstallTemporaryAddon(tbAddonPath.Text);
            tbAddonData.Text = res.AddonId ?? res.Error;
        }

        private async void Button_Click_13(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var res = await ffDriver.Fetch(tbFetchUrl.Text);
            tbFetchRes.Text = res.Result ?? res.Error;
        }

        private async void Button_Click_14(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            var res = await ffDriver.CacheStorage().GetCacheInfo();
            tbCacheDir.Text = res.DiskDirectory;
            lbCacheInfo.ItemsSource = null;
            lbCacheInfo.ItemsSource = res.entries;
        }

        private void lbCacheInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var entry = lbCacheInfo.SelectedItem as CacheEntry;
            if (entry == null) return;
            tbCacheEntryInfo.Text = entry?.GetInfo();

            tbCacheInfoFileName.Text = GetFileNameFromUrl(entry.Url);
        }
        static string GetFileNameFromUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                uri = new Uri(url);

            return Path.GetFileName(uri.LocalPath);
        }

        private async void Button_Click_15(object sender, RoutedEventArgs e)
        {
            var entry = lbCacheInfo.SelectedItem as CacheEntry;
            if (ffDriver == null || entry == null) return;
            var res = await ffDriver.CacheStorage().GetEntryHeaders(entry);
            tbCacheEntryInfo.Text += Environment.NewLine + res;
        }

        private async void Button_Click_16(object sender, RoutedEventArgs e)
        {
            var entry = lbCacheInfo.SelectedItem as CacheEntry;
            if (ffDriver == null || entry == null) return;
            var path = Path.Combine(tbCacheInfoSaveDir.Text, tbCacheInfoFileName.Text);
            var res = await ffDriver.CacheStorage().SaveEntryDataToFile(entry, path);
            tbCacheEntryInfo.Text += Environment.NewLine + (res.Result ?? res.Error);

        }

        private async void Button_Click_17(object sender, RoutedEventArgs e)
        {
            if (ffDriver == null) return;
            await ffDriver.CacheStorage().Clear();
            tbCacheEntryInfo.Text = "Cleared";
        }

        private async void Button_Click_18(object sender, RoutedEventArgs e)
        {
            var entry = lbCacheInfo.SelectedItem as CacheEntry;
            if (ffDriver == null || entry == null) return;
            var res = await ffDriver.CacheStorage().GetEntryData(entry, true);
            tbCacheEntryInfo.Text += Environment.NewLine + (res.Result ?? res.Error);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ffDriver?.CloseSync();
        }
    }
}
