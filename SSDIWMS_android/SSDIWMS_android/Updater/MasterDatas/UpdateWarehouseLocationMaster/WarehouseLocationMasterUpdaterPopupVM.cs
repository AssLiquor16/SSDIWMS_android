using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseLocationMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.MasterDatas.UpdateWarehouseLocationMaster
{
    public class WarehouseLocationMasterUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percalc = new PercentageCalculator();
        IMainServices mainServices;
        ILocalWarehouseLocationMasterServices localDbWarehouseLocationMasterService;
        IServerWarehouseLocationMasterServices serverDbWarehouseLocationMasterService;
        IToastNotifService notifService;

        string _staticloadingText, _loadingText, _taskType, _errorText;

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public ObservableRangeCollection<WarehouseLocationModel> WarehouseLocationList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public WarehouseLocationMasterUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbWarehouseLocationMasterService = DependencyService.Get<ILocalWarehouseLocationMasterServices>();
            serverDbWarehouseLocationMasterService = DependencyService.Get<IServerWarehouseLocationMasterServices>();
            notifService = DependencyService.Get<IToastNotifService>();

            WarehouseLocationList = new ObservableRangeCollection<WarehouseLocationModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            StaticLoadingText = "Updating...";
            await UpdateWarehouseLocationMaster();
        }

        private async Task UpdateWarehouseLocationMaster()
        {
            try
            {
                WarehouseLocationList.Clear();
                var retdata = await serverDbWarehouseLocationMasterService.GetList();
                WarehouseLocationList.AddRange(retdata);
                decimal totcount = WarehouseLocationList.Count;
                decimal foreachcount = 0;
                foreach (var item in retdata)
                {
                    var filter = new WarehouseLocationModel
                    {
                        LocId = item.LocId
                    };
                    var localDataCheck = await localDbWarehouseLocationMasterService.GetFirstOrDefault(filter);
                    if (localDataCheck == null)
                    {
                        await localDbWarehouseLocationMasterService.Insert(item);
                    }
                    else
                    {
                        await localDbWarehouseLocationMasterService.Update(item);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percalc.GetPercentage("WarehouseLocation", decArray);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "WarehouseLocation Master master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
