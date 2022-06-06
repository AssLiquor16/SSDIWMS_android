using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.SiteMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.SiteMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.MasterDatas.UpdateSiteMaster
{
    public class SiteMasterUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percalc = new PercentageCalculator();
        IMainServices mainServices;
        ILocalSiteMasterServices localDbSiteMasterService;
        IServerSiteMasterServices serverDbSiteMasterService;
        IToastNotifService notifService;

        string _staticloadingText, _loadingText, _taskType, _errorText;

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public ObservableRangeCollection<SitesModel> SitesList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public SiteMasterUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbSiteMasterService = DependencyService.Get<ILocalSiteMasterServices>();
            serverDbSiteMasterService = DependencyService.Get<IServerSiteMasterServices>();
            notifService = DependencyService.Get<IToastNotifService>();

            SitesList = new ObservableRangeCollection<SitesModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            StaticLoadingText = "Updating...";
            await UpdateSitesMaster();
        }

        private async Task UpdateSitesMaster()
        {
            try
            {
                SitesList.Clear();
                var retdata = await serverDbSiteMasterService.GetList();
                SitesList.AddRange(retdata);
                decimal totcount = SitesList.Count;
                decimal foreachcount = 0;
                foreach (var item in retdata)
                {
                    int[] intfilterarray = { item.SiteId };
                    var localDataCheck = await localDbSiteMasterService.GetModel("SiteId", item.SiteId);
                    if (localDataCheck == null)
                    {
                        await localDbSiteMasterService.Insert("Common", item);
                    }
                    else
                    {
                        await localDbSiteMasterService.Update("Common", item);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percalc.GetPercentage("Sites", decArray);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "Site master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
