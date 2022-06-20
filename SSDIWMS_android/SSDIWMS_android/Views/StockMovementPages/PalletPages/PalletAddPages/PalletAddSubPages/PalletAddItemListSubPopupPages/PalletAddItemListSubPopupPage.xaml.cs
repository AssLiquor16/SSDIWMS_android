using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs.PalletAddItemListSubPopupVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.PalletPages.PalletAddPages.PalletAddSubPages.PalletAddItemListSubPopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletAddItemListSubPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PalletAddItemListSubPopupPage(object obj = null)
        {
            InitializeComponent();
            var con = BindingContext as PalletAddItemListSubPopupVM;
            con.PassedItem = (obj as ItemMasterModel);
        }
    }
}