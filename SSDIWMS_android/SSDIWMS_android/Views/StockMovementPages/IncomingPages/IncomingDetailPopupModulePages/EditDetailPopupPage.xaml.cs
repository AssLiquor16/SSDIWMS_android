using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDetailPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public EditDetailPopupPage(IncomingPartialDetailModel e)
        {
            InitializeComponent();
            var a = BindingContext as EditDetailPopupVM;
            a.E = e;
            a.PONumber = e.POHeaderNumber;
            a.ItemCode = e.ItemCode;
            a.ItemDesc = e.ItemDesc;
            a.PalletCode = e.PalletCode;
            a.PartialCQTY = e.PartialCQTY;
            a.ExpiryDate = e.ExpiryDate;
        }
    }
}