using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingHeaderPage : ContentPage
    {
        public IncomingHeaderPage()
        {
            InitializeComponent();
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var con = BindingContext as IncomingHeaderVM;
            var n = con.DummyIncomingHeaderList.Where(x => x.IsSelected == true).ToList();
            if(n.Count > 0)
            {
                con.SummaryBtnVisible = true;
            }
            else
            {
                con.SummaryBtnVisible=false;
            }
        }
    }
}