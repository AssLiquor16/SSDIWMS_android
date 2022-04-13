using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingPopUpSubVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingPopUpSubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingDetailAddPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public IncomingDetailAddPopupPage(string[] details)
        {
            // [0] PO number
            // [1] Case code
            InitializeComponent();
            var myViewModel = BindingContext as IncomingDetailAddPopupVM;
            myViewModel.PONumber = details[0];
            myViewModel.ScannedCode = details[1];
        }

    }
}