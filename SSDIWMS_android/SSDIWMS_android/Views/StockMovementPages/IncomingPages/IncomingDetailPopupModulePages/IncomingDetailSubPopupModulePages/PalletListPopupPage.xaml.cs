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
    public partial class PalletListPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PalletListPopupPage(string requester)
        {
            InitializeComponent();
            var myViewModel = BindingContext as PalletListPopupVM;
            myViewModel.PageCameFrom = requester;
        }
    }
}