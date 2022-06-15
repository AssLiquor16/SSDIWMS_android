using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.MasterDatas.UpdateAllMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdateAllUserMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdateArticleMaster;
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
        public LiveTime livetime { get; } = new LiveTime();
        IToastNotifService notifyService;
        IMainServices mainService;
        bool _notifyIO, _adminViewVisible;
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
        public AsyncCommand NotifCommand { get; }
        public AsyncCommand ArticleMasterUpdateNavCommand { get; }
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
            NotifCommand = new AsyncCommand(Notif);
            ArticleMasterUpdateNavCommand = new AsyncCommand(ArticleMasterUpdateNav);
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
        private async Task ArticleMasterUpdateNav() => await PopupNavigation.Instance.PushAsync(new ArticleMasterPickUpdaterPopupPage());
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
            await livetime.LiveTimer();
            NotifyIO = Preferences.Get("NotifyIO", true);
            switch (Preferences.Get("PrefUserRole",string.Empty))
            {
                case "Admin":
                    AdminViewVisible = true;
                    break;
                default: AdminViewVisible = false; break;
            }

        }
    }
}
