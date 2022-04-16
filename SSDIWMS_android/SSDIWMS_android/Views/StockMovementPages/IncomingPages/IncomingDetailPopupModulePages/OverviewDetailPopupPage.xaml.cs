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
    public partial class OverviewDetailPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public OverviewDetailPopupPage(string PO)
        {
            InitializeComponent();
            var myVM = BindingContext as OverviewDetailPopupVM;
            myVM.PONumber = PO;
        }
    }
}