using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.PutAwayPages;
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
        public AsyncCommand PutAwayPageNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public STTPalletToLocTransactionTypeVM()
        {
            PutAwayPageNavCommand = new AsyncCommand(PutAwayPageNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PutAwayPageNav()
        {
            await Shell.Current.GoToAsync($"{nameof(PHTransferFromPage)}");
        }

        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
        }
    }
}
