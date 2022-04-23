using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailModuleVMs.IncomingDetailSubModuleVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailModulePages.IncomingDetailSubModulePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletMasterListDetailSubModulePage : ContentPage
    {
        ZXingScannerPage scanPage;
        public PalletMasterListDetailSubModulePage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            var con = BindingContext as PalletMasterListDetailSubModuleVM;
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                    con.SearchString = result.Text;
                });

            };
            await Navigation.PushAsync(scanPage);
        }
    }
}