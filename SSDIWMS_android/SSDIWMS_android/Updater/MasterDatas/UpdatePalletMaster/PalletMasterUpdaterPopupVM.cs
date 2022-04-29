using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.MasterDatas.UpdatePalletMaster
{
    public class PalletMasterUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percalc = new PercentageCalculator();
        IMainServices mainServices;
        ILocalPalletMasterServices localDbPalletMasterService;
        IServerPalletMasterServices serverDbPalletMasterService;
        IToastNotifService notifService;

        string _staticloadingText, _loadingText, _taskType, _errorText;

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public ObservableRangeCollection<PalletMasterModel> PalletMasterList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletMasterUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbPalletMasterService = DependencyService.Get<ILocalPalletMasterServices>();
            serverDbPalletMasterService = DependencyService.Get<IServerPalletMasterServices>();
            notifService = DependencyService.Get<IToastNotifService>();

            PalletMasterList = new ObservableRangeCollection<PalletMasterModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            StaticLoadingText = "Updating...";
            await UpdatePalletMaster();
        }

        private async Task UpdatePalletMaster()
        {
            try
            { 
                PalletMasterList.Clear();
                var retdata = await serverDbPalletMasterService.GetList("All", null, null);
                PalletMasterList.AddRange(retdata);
                decimal totcount = PalletMasterList.Count;
                decimal foreachcount = 0;
                foreach (var item in retdata)
                {
                    int[] intfilterarray = { item.PId };
                    var localDataCheck = await localDbPalletMasterService.GetModel("PId", null, intfilterarray);
                    if (localDataCheck == null)
                    {
                        await localDbPalletMasterService.Insert("Common", item);
                    }
                    else
                    {
                        await localDbPalletMasterService.Update("Common", item);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percalc.GetPercentage("PalletMaster", decArray);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "Pallet master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await PopupNavigation.Instance.PopAsync(true);
        }

    }
}
