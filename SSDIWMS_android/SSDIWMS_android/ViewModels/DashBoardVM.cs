using MvvmHelpers.Commands;
using SSDIWMS_android.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels
{
    public class DashBoardVM : ViewModelBase
    {

        public AsyncCommand PageRefreshCommand { get; }
        public DashBoardVM()
        {
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task PageRefresh()
        {
            bool login = Preferences.Get("PrefLoggedIn", false);
            if (login == false)
            {
                var route = $"//{nameof(LoginPage)}";
                await Shell.Current.GoToAsync(route);
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
