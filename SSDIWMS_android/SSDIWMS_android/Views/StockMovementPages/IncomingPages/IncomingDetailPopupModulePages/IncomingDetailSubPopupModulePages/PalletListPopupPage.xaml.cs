using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs.IncomingDetailSubPopupModuleVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages.IncomingDetailSubPopupModulePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletListPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        ZXingScannerPage scanPage;
        public PalletListPopupPage(string requester)
        {
            InitializeComponent();
            var myViewModel = BindingContext as PalletListPopupVM;
            myViewModel.PageCameFrom = requester;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            var con = BindingContext as PalletListPopupVM;
            scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();
                        con.SearchString = result.Text;
                        await con.Back();
                    });

                };
                await Navigation.PushAsync(scanPage);
                await con.Close();
        }
    }
}