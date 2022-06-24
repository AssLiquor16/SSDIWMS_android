using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs
{
    public class STTransferTypesVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();

        public AsyncCommand STPalletToLocTransferTypePageNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public STTransferTypesVM()
        {
            STPalletToLocTransferTypePageNavCommand = new AsyncCommand(STPalletToLocTransferTypePageNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task STPalletToLocTransferTypePageNav() => await Shell.Current.GoToAsync($"{nameof(STTPalletToLocTransactionTypePage)}");


        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
        }
    }
}
