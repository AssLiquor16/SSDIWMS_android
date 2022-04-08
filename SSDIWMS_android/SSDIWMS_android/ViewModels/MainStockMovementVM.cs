using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Views.PopUpPages;
using SSDIWMS_android.Views.StockMovementPages;
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
        bool _proceedEnable;
        public bool ProceedEnable { get => _proceedEnable; set => SetProperty(ref _proceedEnable, value); }

        public AsyncCommand NavigationCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public MainStockMovementVM()
        {
            NavigationCommand = new AsyncCommand(Navigation);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Navigation()
        {
            var route = $"{nameof(HeaderPage)}";
            await Shell.Current.GoToAsync(route);
        }
        private async Task PageRefresh()
        {
            
            var role = Preferences.Get("PrefUserRole", string.Empty);
            if(role == "Pick" || role == "Check")
            {
                ProceedEnable = true;
            }
            else
            {
                ProceedEnable = false;
            }
            await LiveTimer();
        }

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt");
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        private async Task LiveTimer()
        {
            await Task.Delay(1);
            Device.StartTimer(TimeSpan.FromSeconds(_datetimeTick), () => {
                Task.Run(async () =>
                {
                    await Task.Delay(1);
                    LiveDate = DateTime.Now.ToString(_datetimeFormat);
                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }
    }
}
