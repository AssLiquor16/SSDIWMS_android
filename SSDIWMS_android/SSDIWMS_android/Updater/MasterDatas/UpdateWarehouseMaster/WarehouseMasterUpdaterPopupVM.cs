using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.MasterDatas.UpdateWarehouseMaster
{
    public class WarehouseMasterUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percalc = new PercentageCalculator();
        IMainServices mainServices;
        ILocalWarehouseMasterServices localDbWarehouseMasterService;
        IServerWarehouseMasterServices serverDbWarehouseMasterService;
        IToastNotifService notifService;

        string _staticloadingText, _loadingText, _taskType, _errorText;

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public ObservableRangeCollection<WarehouseModel> WarehouseList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public WarehouseMasterUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbWarehouseMasterService = DependencyService.Get<ILocalWarehouseMasterServices>();
            serverDbWarehouseMasterService = DependencyService.Get<IServerWarehouseMasterServices>();
            notifService = DependencyService.Get<IToastNotifService>();

            WarehouseList = new ObservableRangeCollection<WarehouseModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            StaticLoadingText = "Updating...";
            await UpdateWarehouseMaster();
        }

        private async Task UpdateWarehouseMaster()
        {
            try
            {
                WarehouseList.Clear();
                var retdata = await serverDbWarehouseMasterService.GetList();
                WarehouseList.AddRange(retdata);
                decimal totcount = WarehouseList.Count;
                decimal foreachcount = 0;
                foreach (var item in retdata)
                {
                    var filter = new WarehouseModel
                    {
                        WarehouseId = item.WarehouseId
                    };
                    var localDataCheck = await localDbWarehouseMasterService.GetFirstOrDefault(filter);
                    if (localDataCheck == null)
                    {
                        await localDbWarehouseMasterService.Insert(item);
                    }
                    else
                    {
                        await localDbWarehouseMasterService.Update(item);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percalc.GetPercentage("Warehouse", decArray);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "Warehouse Master master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}

