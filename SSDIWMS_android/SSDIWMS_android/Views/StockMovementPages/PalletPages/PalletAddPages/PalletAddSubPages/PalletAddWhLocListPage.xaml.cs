using SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SSDIWMS_android.Views.StockMovementPages.PalletPages.PalletAddPages.PalletAddSubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletAddWhLocListPage : ContentPage
    {
        public PalletAddWhLocListPage()
        {
            InitializeComponent();
        }

        ZXingScannerPage scanPage;
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            var con = BindingContext as PalletAddWhLocListVM;
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    con.ApiSearchCode = result.Text;
                    await Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(scanPage);
        }
    }
}