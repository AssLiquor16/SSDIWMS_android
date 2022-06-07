using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.StockMovementPages.IncomingPages.BatchGeneratePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BatchGenPOListPage : ContentPage
    {
        public BatchGenPOListPage()
        {
            InitializeComponent();
        }

        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var con = BindingContext as BatchGenPOListVM;
            await con.TotalAllSelected();
        }
    }
}