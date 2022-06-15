using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Views.PalletPages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages;
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
        bool _proceedEnable, _createPalletBtnVisible;
        public bool ProceedEnable { get => _proceedEnable; set => SetProperty(ref _proceedEnable, value); }
        public bool CreatePalletBtnVisible { get => _createPalletBtnVisible; set => SetProperty(ref _createPalletBtnVisible, value); }
        public AsyncCommand PalletHeaderNavCommand { get; }
        public AsyncCommand IncomingNavigationCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public MainStockMovementVM()
        {
            PalletHeaderNavCommand = new AsyncCommand(PalletHeadernav);
            IncomingNavigationCommand = new AsyncCommand(IncomingNavigation);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task PalletHeadernav() => await Shell.Current.GoToAsync($"{nameof(PalletHeaderPage)}"); 



        private async Task IncomingNavigation()
        {
            var route = $"{nameof(IncomingHeaderPage)}";
            Preferences.Remove("PrefIncomingHeaderPagepartialRefresh");
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
                }
                else if(role == "Pick")
                {
                    CreatePalletBtnVisible = true;
                }
                ProceedEnable = true;
            }
            else
            {
                ProceedEnable = false;
                CreatePalletBtnVisible = false;
            }
            await livetime.LiveTimer();
            
        }
    }
}
