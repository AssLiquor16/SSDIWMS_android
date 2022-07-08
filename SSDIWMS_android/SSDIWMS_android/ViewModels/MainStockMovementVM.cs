using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.PurchaseOrderPages2;
using SSDIWMS_android.Views.StockMovementPages.PalletPages;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels
{
    public class MainStockMovementVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        bool _proceedEnable, _createPalletBtnVisible, _stockTransferBtnVisible;
        public bool ProceedEnable { get => _proceedEnable; set => SetProperty(ref _proceedEnable, value); }
        public bool CreatePalletBtnVisible { get => _createPalletBtnVisible; set => SetProperty(ref _createPalletBtnVisible, value); }
        public bool StockTransferBtnVisible { get => _stockTransferBtnVisible; set => SetProperty(ref _stockTransferBtnVisible, value); }
        public AsyncCommand StockTransferNavCommand { get; }
        public AsyncCommand PalletHeaderNavCommand { get; }
        public AsyncCommand NewIncomingNavCommand { get; }
        public AsyncCommand IncomingNavigationCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public MainStockMovementVM()
        {
            StockTransferNavCommand = new AsyncCommand(StockTransferNav);
            PalletHeaderNavCommand = new AsyncCommand(PalletHeadernav);
            NewIncomingNavCommand = new AsyncCommand(NewIncomingNav);
            IncomingNavigationCommand = new AsyncCommand(IncomingNavigation);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task NewIncomingNav() => await Shell.Current.GoToAsync($"{nameof(BillDocListPage)}");
        private async Task StockTransferNav() => await Shell.Current.GoToAsync($"{nameof(STTransferTypesPage)}");
        private async Task PalletHeadernav() => await Shell.Current.GoToAsync($"{nameof(PalletHeaderListPage)}");

        private async Task IncomingNavigation()
        {
            var route = $"{nameof(IncomingTypesPage)}";
            await Shell.Current.GoToAsync(route);
        }
        private async Task PageRefresh()
        {
            
            var role = Preferences.Get("PrefUserRole", string.Empty);
            if(role == "Pick" || role == "Check")
            {
                if(role == "Check")
                {
                    CreatePalletBtnVisible = false;
                    StockTransferBtnVisible = false;
                }
                else if(role == "Pick")
                {
                    CreatePalletBtnVisible = true;
                    StockTransferBtnVisible = true;
                }
                ProceedEnable = true;
            }
            else
            {
                ProceedEnable = false;
                CreatePalletBtnVisible = false;
                StockTransferBtnVisible = false;
            }
            await livetime.LiveTimer();
            
        }
    }
}
