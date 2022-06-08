using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs.BatchPopupVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.BatchGeneratePages.BatchPopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BatchGenPOListPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public BatchGenPOListPopupPage()
        {
            InitializeComponent();
        }

        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var con = BindingContext as BatchGenPOListPopupVM;
            await con.TotalAllSelected();
        }
    }
}