using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.SiteMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.SiteMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.UpdateArticleMaster
{
    public class ArticleMasterUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percentageCalculator = new PercentageCalculator();
        IMainServices mainServices;
        ILocalArticleMasterServices localDbArticleMasterService;
        IServerArticleMasterServices serverDbArticleMasterService;
        ILocalSiteMasterServices localDbSiteMasterService;
        IServerSiteMasterServices serverDbSiteMasterService;
        IToastNotifService notifService;

        string _staticloadingText, _loadingText, _taskType, _errorText;

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public ObservableRangeCollection<ArticleMasterModel> ArticleMasterList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public ArticleMasterUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbArticleMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            serverDbArticleMasterService = DependencyService.Get<IServerArticleMasterServices>();

            localDbSiteMasterService = DependencyService.Get<ILocalSiteMasterServices>();
            serverDbSiteMasterService = DependencyService.Get<IServerSiteMasterServices>();

            notifService = DependencyService.Get<IToastNotifService>();

            ArticleMasterList = new ObservableRangeCollection<ArticleMasterModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            StaticLoadingText = "Updating...";
            await UpdateArticleMaster();
        }

        private async Task UpdateArticleMaster()
        {
            try
            {

                var sites = await serverDbSiteMasterService.GetList("Common", null, null);
                foreach (var site in sites)
                {
                    var locsite = await localDbSiteMasterService.GetModel("Common", site.SiteId);
                    if (locsite == null)
                    {
                        await localDbSiteMasterService.Insert("Common", site);
                    }
                    else
                    {
                        await localDbSiteMasterService.Update("Common", site);
                    }
                }
                ArticleMasterList.Clear();
                var retdata = await serverDbArticleMasterService.GetList("All", null, null);
                ArticleMasterList.AddRange(retdata);
                decimal totcount = ArticleMasterList.Count;
                decimal foreachcount = 0;
                foreach (var item in retdata)
                {
                    int[] intfilterarray = { item.Id };
                    var cat1 = "";
                    switch (item.Assortment_No)
                    {
                        case "PH_HPCFR":
                            cat1 = "HPC";
                            break;
                        case "PH_IC":
                            cat1 = "URIC";
                            break;
                        case "PH_UFS":
                            cat1 = "UFS";
                            break;
                        default: break;
                    }
                    var localDataCheck = await localDbArticleMasterService.GetModel("ItemId", null, intfilterarray);
                    var content = new ItemMasterModel
                    {
                        ItemId = item.Id,
                        ItemCode = item.Article_Code,
                        ItemDesc = item.Article_Description,
                        ItemCat1 = cat1,
                        ItemCat2 = item.Category,
                        Barcode = item.Barcode,
                        CaseCode = item.Casecode,
                        Status = item.Status
                    };
                    if (localDataCheck == null)
                    {
                        await localDbArticleMasterService.Insert("Common", content);
                    }
                    else
                    {
                        await localDbArticleMasterService.Update("Common", content);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percentageCalculator.GetPercentage("ArticleMaster", decArray);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "Article master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await PopupNavigation.Instance.PopAllAsync(true);
        }

    }
}
