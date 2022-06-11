using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.MasterDatas.UpdateAllMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdateAllUserMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdatePalletMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdateSiteMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdateWarehouseLocationMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdateWarehouseMaster;
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
        bool _notifyIO, _adminViewVisible;
        string _ipVal;
        public bool NotifyIO
        {
            get => _notifyIO;
            set
            {

                if (value == _notifyIO)
                    return;
                Preferences.Set("NotifyIO", value);
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
        public AsyncCommand AreaMasterUpdateCommand { get; }
        public AsyncCommand WarehouseLocationUpdateCommand { get; }
        public AsyncCommand WarehouseUpdateCommand { get; }
        public AsyncCommand UserMasterUpdateCommand { get; }
        public AsyncCommand AllUpdateCommand { get; }
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
            AreaMasterUpdateCommand = new AsyncCommand(AreaMasterUpdate);
            WarehouseLocationUpdateCommand = new AsyncCommand(WarehouseLocationUpdate);
            WarehouseUpdateCommand = new AsyncCommand(WarehouseUpdate);
            UserMasterUpdateCommand = new AsyncCommand(UserMasterUpdate);
            AllUpdateCommand = new AsyncCommand(AllUpdate);
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
        
        private async Task ArticleMasterUpdate() => await PopupNavigation.Instance.PushAsync(new ArticleMasterUpdaterPopupPage());
        private async Task PalletMasterUpdate() => await PopupNavigation.Instance.PushAsync(new PalletMasterUpdaterPopupPage());
        private async Task AreaMasterUpdate() => await PopupNavigation.Instance.PushAsync(new SiteMasterUpdaterPopupPage());
        private async Task WarehouseLocationUpdate() => await PopupNavigation.Instance.PushAsync(new WarehouseLocationMasterUpdaterPopupPage());
        private async Task WarehouseUpdate() => await PopupNavigation.Instance.PushAsync(new WarehouseMasterUpdaterPopupPage());
        private async Task UserMasterUpdate() => await PopupNavigation.Instance.PushAsync(new UserMasterUpdaterPopupPage());
        private async Task AllUpdate() => await PopupNavigation.Instance.PushAsync(new AllMasterfileUpdaterPopupPage());
        private async Task ClearTransaction() => await PopupNavigation.Instance.PushAsync(new FormPopupPage("AdminClearTrans"));
        private async Task PageRefresh()
        {
            await LiveTimer();
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
            Role = Preferences.Get("PrefUserRole", "");
            NotifyIO = Preferences.Get("NotifyIO", true);
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
