﻿using Rg.Plugins.Popup.Services;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingDetailListPage : ContentPage
    {
        public IncomingDetailListPage()
        {
            InitializeComponent();
        }

    }
}
// await PopupNavigation.Instance.PushAsync(new IncomingDetailAddPopupPage());