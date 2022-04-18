using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs.IncomingDetailSubPopupModuleVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages.IncomingDetailSubPopupModulePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartialDetailListPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PartialDetailListPopupPage(IncomingDetailModel item)
        {
            InitializeComponent();
            var myVM = BindingContext as PartialDetailListPopupVM;
            myVM.PassedItem = item;
        }
    }
}