using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.PutAwayPages;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.StockMovementPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs
{
    public class STTPalletToLocTransactionTypeVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        public AsyncCommand StockMovementPageNavCommand { get; }
        public AsyncCommand PutAwayPageNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public STTPalletToLocTransactionTypeVM()
        {
            StockMovementPageNavCommand = new AsyncCommand(StockMovementPageNav);
            PutAwayPageNavCommand = new AsyncCommand(PutAwayPageNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task StockMovementPageNav() => await Shell.Current.GoToAsync($"{nameof(SMPHTrasnferFromPage)}");
        private async Task PutAwayPageNav() => await Shell.Current.GoToAsync($"{nameof(PHTransferFromPage)}");

        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
        }
    }
}
