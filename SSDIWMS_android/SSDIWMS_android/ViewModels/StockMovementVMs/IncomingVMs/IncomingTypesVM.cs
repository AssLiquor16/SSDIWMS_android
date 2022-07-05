using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs
{
    public class IncomingTypesVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        public AsyncCommand PurchaseOrderNavCommand { get; }
        public AsyncCommand BadStocksNavCommand { get; }
        public AsyncCommand ReturnGoodStocksNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }

        public IncomingTypesVM()
        {
            BadStocksNavCommand = new AsyncCommand(BadStocksNav);
            ReturnGoodStocksNavCommand = new AsyncCommand(ReturnGoodStocksNav);
            PurchaseOrderNavCommand = new AsyncCommand(PurchaseOrderNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task ReturnGoodStocksNav()
        {

        }
        private async Task BadStocksNav()
        {

        }
        private async Task PurchaseOrderNav()
        {
            Preferences.Remove("PrefIncomingHeaderPagepartialRefresh");
            var route = $"{nameof(IncomingHeaderPage)}";
            await Shell.Current.GoToAsync(route);
        }
        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
        }
    }
}
