﻿using Rg.Plugins.Popup.Services;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingDetailListPage : ContentPage
    {
        ZXingScannerPage scanPage;
        public IncomingDetailListPage()
        {
            InitializeComponent();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            
            scanPage = new ZXingScannerPage();
            var con = BindingContext as IncomingDetailListVM;
            if(con.Role == "Check")
            {
                scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();
                        await con.AddPopupNav(result.Text);

                    });

                };
                await Navigation.PushAsync(scanPage);
            }
            else
            {

            }
            
        }
    }
}
// await PopupNavigation.Instance.PushAsync(new IncomingDetailAddPopupPage());