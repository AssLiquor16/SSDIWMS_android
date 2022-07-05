using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.SummaryPopupModuleVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.SummaryPopupModulePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SummaryPopupSubPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public SummaryPopupSubPage(string[] Pos)
        {
            InitializeComponent();
            var con = BindingContext as SummaryPopupSubVM;
            con.POS = Pos;
        }
    }
}