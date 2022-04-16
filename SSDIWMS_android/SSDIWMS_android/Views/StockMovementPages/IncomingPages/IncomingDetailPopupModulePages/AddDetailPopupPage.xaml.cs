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
    public partial class AddDetailPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        
        public AddDetailPopupPage(string[] filters)
        {
            InitializeComponent();
            var myViewModel = BindingContext as AddDetailPopupVM;
            myViewModel.PONumber = filters[0];
            myViewModel.ScannedCode = filters[1];
        }

    }
}