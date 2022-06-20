using SSDIWMS_android.Models.SMTransactionModel.Temp;
using SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubPopupVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.PalletPages.PalletAddPages.PalletAddSubPopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletAddItemDetailPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PalletAddItemDetailPopupPage(ItemWithQtyModel obj)
        {
            InitializeComponent();
            var con = BindingContext as PalletAddItemDetailPopupVM;
            con.PasseItem = obj;
        }

        
    }
}