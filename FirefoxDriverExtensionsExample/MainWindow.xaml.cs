using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
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
            if (FirefoxProfilesWorker.GetMarionettePort(profileName) == 0)
                FirefoxProfilesWorker.SetMarionettePort(profileName, 5432);
            FirefoxProfilesWorker.OpenFirefoxProfile(profileName);

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
    }
}
