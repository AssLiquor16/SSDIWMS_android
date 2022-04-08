using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Views.PopUpPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels
{
    public class SettingVM : ViewModelBase
    {
        bool _notifyIO = Preferences.Get("NotifyIO", false);
        public bool NotifyIO
        {
            get => _notifyIO;
            set
            {

                if (value == _notifyIO)
                    return;
                _notifyIO = value;
                OnPropertyChanged();
            }
        }

        public AsyncCommand NotifCommand { get; }
        public AsyncCommand ArticleMasterUpdateCommand { get; }
        public AsyncCommand PalletMasterUpdateCommand { get; }
        public AsyncCommand ClearTransactionCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }


        public SettingVM()
        {
            NotifCommand = new AsyncCommand(Notif);
            ArticleMasterUpdateCommand = new AsyncCommand(ArticleMasterUpdate);
            PalletMasterUpdateCommand = new AsyncCommand(PalletMasterUpdate);
            ClearTransactionCommand = new AsyncCommand(ClearTransaction);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task Notif()
        {
            var e = Preferences.Get("NotifyIO", false);
            if (e == false)
            {
                Preferences.Set("NotifyIO", true);
            }
            else
            {
                Preferences.Set("NotifyIO", false);
            }

            NotifyIO = Preferences.Get("NotifyIO", false);
            await Task.Delay(1);
        }
        
        private async Task ArticleMasterUpdate()
        {
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage("UpdateArticleMaster"));
        }
        private async Task PalletMasterUpdate()
        {
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage("UpdatePalletMaster"));
        }
        private async Task ClearTransaction()
        {
            await PopupNavigation.Instance.PushAsync(new FormPopupPage("AdminClearTrans"));
        }
        private async Task PageRefresh()
        {
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
