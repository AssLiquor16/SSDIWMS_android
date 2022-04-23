using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.MasterDatas.UpdatePalletMaster;
using SSDIWMS_android.Updater.UpdateArticleMaster;
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
        IToastNotifService notifyService;
        IMainServices mainService;
        bool _notifyIO = Preferences.Get("NotifyIO", false), _adminViewVisible;
        string _ipVal;
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
        public bool AdminViewVisible { get => _adminViewVisible; set => SetProperty(ref _adminViewVisible, value); }
        public string IPVal { get => _ipVal; set => SetProperty(ref _ipVal, value); }
       
        public AsyncCommand SaveIpAddressCommand { get; }
        public AsyncCommand ReturnDefaultIpCommand { get; }
        public AsyncCommand NotifCommand { get; }
        public AsyncCommand ArticleMasterUpdateCommand { get; }
        public AsyncCommand PalletMasterUpdateCommand { get; }
        public AsyncCommand ClearTransactionCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }


        public SettingVM()
        {
            mainService = DependencyService.Get<IMainServices>();
            notifyService = DependencyService.Get<IToastNotifService>();
            SaveIpAddressCommand = new AsyncCommand(SaveIpAddress);
            ReturnDefaultIpCommand = new AsyncCommand(ReturnDefaultIp);
            NotifCommand = new AsyncCommand(Notif);
            ArticleMasterUpdateCommand = new AsyncCommand(ArticleMasterUpdate);
            PalletMasterUpdateCommand = new AsyncCommand(PalletMasterUpdate);
            ClearTransactionCommand = new AsyncCommand(ClearTransaction);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task SaveIpAddress()
        {
            Preferences.Remove("PrefServerAddress");
            if (!string.IsNullOrWhiteSpace(IPVal))
            {
                bool isProceed = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to change the IP address?", "Yes", "No");
                if(isProceed == true)
                {
                    Preferences.Set("PrefServerAddress", IPVal);
                    await notifyService.StaticToastNotif("Success", "I.P address save.");
                    await Task.Delay(2000);
                    await mainService.RemovePreferences();
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                }
                
            }
            else
            {
                await notifyService.StaticToastNotif("Error", "Empty entry.");
            }
            
        }
        private async Task ReturnDefaultIp()
        {
            Preferences.Remove("PrefServerAddress");
            IPVal = "http://192.168.1.217:80/";
            Preferences.Set("PrefServerAddress", IPVal);
            await notifyService.StaticToastNotif("Success", "I.P address reset to default.");
            await Task.Delay(2000);
            await mainService.RemovePreferences();
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
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
            await PopupNavigation.Instance.PushAsync(new ArticleMasterUpdaterPopupPage());
        }
        private async Task PalletMasterUpdate()
        {
            await PopupNavigation.Instance.PushAsync(new PalletMasterUpdaterPopupPage());
        }
        private async Task ClearTransaction()
        {
            await PopupNavigation.Instance.PushAsync(new FormPopupPage("AdminClearTrans"));
        }
        private async Task PageRefresh()
        {
            await LiveTimer();
            UserFullName = Preferences.Get("PrefUserFullname", "");
            Role = Preferences.Get("PrefUserRole", "");
            switch (Role)
            {
                case "Admin":
                    AdminViewVisible = true;
                    break;
                default: AdminViewVisible = false; break;
            }
            IPVal = Preferences.Get("PrefServerAddress", "http://192.168.1.217:80/");

        }

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt"), _userFullname,_role;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
        public string Role { get => _role; set => SetProperty(ref _role, value); }
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
