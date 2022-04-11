using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingSubVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingSubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingDetailAddPage : ContentPage
    {
        ZXingScannerPage scanPage;
        public IncomingDetailAddPage()
        {
            InitializeComponent();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            var con = BindingContext as IncomingDetailAddVM;
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(() =>
                {
                    con.BarcodeVal = result.Text;
                    Navigation.PopAsync();

                });

            };
            await Navigation.PushAsync(scanPage);
        }
    }
}