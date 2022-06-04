using MvvmHelpers.Commands;
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
        bool _proceedEnable;
        public bool ProceedEnable { get => _proceedEnable; set => SetProperty(ref _proceedEnable, value); }
        public AsyncCommand IncomingNavigationCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public MainStockMovementVM()
        {
            IncomingNavigationCommand = new AsyncCommand(Navigation);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Navigation()
        {
            var route = $"{nameof(IncomingHeaderPage)}";
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
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
            
        }

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt"), _userFullname;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
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
