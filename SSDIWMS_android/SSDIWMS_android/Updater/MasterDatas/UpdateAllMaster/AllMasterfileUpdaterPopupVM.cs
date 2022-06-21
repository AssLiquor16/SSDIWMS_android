using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.SiteMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.UserMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.SiteMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseLocationMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Services.ServerDbServices.Users;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.MasterDatas.UpdateAllMaster
{
    public class AllMasterfileUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percalc = new PercentageCalculator();
        IMainServices mainServices;
        ILocalUserServices localDbUserMasterService;
        IServerUserServices serverDbUserMasterService;
        ILocalArticleMasterServices localDbArticleMasterService;
        IServerArticleMasterServices serverDbArticleMasterService;
        ILocalPalletMasterServices localDbPalletMasterService;
        IServerPalletMasterServices serverDbPalletMasterService;
        IServerSiteMasterServices serverDbSitesMasterService;
        ILocalSiteMasterServices localDbSitesMasterService;
        ILocalWarehouseLocationMasterServices localDbWarehouseLocationMasterMasterService;
        IServerWarehouseLocationMasterServices serverDbWarehouseLocationMasterService;
        ILocalWarehouseMasterServices localDbWarehouseMasterService;
        IServerWarehouseMasterServices serverDbWarehouseMasterService;
        IToastNotifService notifService;

        string _staticloadingText, _loadingText, _taskType, _errorText;

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public ObservableRangeCollection<UsermasterModel> UserMasterList { get; set; }
        public ObservableRangeCollection<ArticleMasterModel> ArticleMasterList { get; set; }
        public ObservableRangeCollection<PalletMasterModel> PalletMasterList { get; set; }
        public ObservableRangeCollection<SitesModel> SitesList { get; set; }
        public ObservableRangeCollection<WarehouseLocationModel> WarehouseLocationList { get; set; }
        public ObservableRangeCollection<WarehouseModel> WarehouseList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        
        public AllMasterfileUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbUserMasterService = DependencyService.Get<ILocalUserServices>();
            serverDbUserMasterService = DependencyService.Get<IServerUserServices>();

            localDbArticleMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            serverDbArticleMasterService = DependencyService.Get<IServerArticleMasterServices>();
            localDbPalletMasterService = DependencyService.Get<ILocalPalletMasterServices>();
            serverDbPalletMasterService = DependencyService.Get<IServerPalletMasterServices>();
            serverDbSitesMasterService = DependencyService.Get<IServerSiteMasterServices>();
            localDbSitesMasterService = DependencyService.Get<ILocalSiteMasterServices>();
            localDbWarehouseLocationMasterMasterService = DependencyService.Get<ILocalWarehouseLocationMasterServices>();
            serverDbWarehouseLocationMasterService = DependencyService.Get<IServerWarehouseLocationMasterServices>();
            localDbWarehouseMasterService = DependencyService.Get<ILocalWarehouseMasterServices>();
            serverDbWarehouseMasterService = DependencyService.Get<IServerWarehouseMasterServices>();
            notifService = DependencyService.Get<IToastNotifService>();

            UserMasterList = new ObservableRangeCollection<UsermasterModel>();
            ArticleMasterList = new ObservableRangeCollection<ArticleMasterModel>();
            PalletMasterList = new ObservableRangeCollection<PalletMasterModel>();
            WarehouseLocationList = new ObservableRangeCollection<WarehouseLocationModel>();
            SitesList = new ObservableRangeCollection<SitesModel>();
            WarehouseList = new ObservableRangeCollection<WarehouseModel>();

            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        public async Task PageRefresh()
        {
            StaticLoadingText = "Processing...";
            await UpdateUserMaster();
            await UpdateSiteMaster();
            await UpdateArticleMaster();
            await UpdateWarehouseMaster();
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async Task UpdateUserMaster()
        {
            try
            {
                UserMasterList.Clear();
                var retdata = await serverDbUserMasterService.ReturnList("All", null, null);
                UserMasterList.AddRange(retdata);
                decimal totcount = UserMasterList.Count;
                decimal foreachcount = 0;
                foreach (var item in retdata)
                {
                    var localDataCheck = await localDbUserMasterService.GetFirstOrDefault(item);
                    if (localDataCheck == null)
                    {
                        await localDbUserMasterService.Insert(item);
                    }
                    else
                    {
                        await localDbUserMasterService.Update(item);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percalc.GetPercentage("PalletMaster", decArray);
                    await Task.Delay(50);
                }
                await notifService.ToastNotif("Success", "User master downloaded succesfully.");
            }
            catch
            {
                await notifService.ToastNotif("Error", ErrorText);
            }
        }
        private async Task UpdateArticleMaster()
        {
            try
            {
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
                        Status = item.Status,
                        Brand = item.Brand,
                        Category = item.Category,
                        Division = item.Division,
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
                    LoadingText = await percalc.GetPercentage("ArticleMaster", decArray);
                    await Task.Delay(50);
                }
                await notifService.ToastNotif("Success", "Article master downloaded succesfully.");
            }
            catch
            {
                await notifService.ToastNotif("Error", ErrorText);
            }
        }
        private async Task UpdateSiteMaster()
        {
            try
            {
                SitesList.Clear();
                var retdata = await serverDbSitesMasterService.GetList();
                SitesList.AddRange(retdata);
                decimal totcount = SitesList.Count;
                decimal foreachcount = 0;
                foreach (var item in retdata)
                {
                    var localDataCheck = await localDbSitesMasterService.GetModel("SiteId", item.SiteId);
                    if (localDataCheck == null)
                    {
                        await localDbSitesMasterService.Insert("Common", item);
                    }
                    else
                    {
                        await localDbSitesMasterService.Update("Common", item);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percalc.GetPercentage("Sites", decArray);
                    await Task.Delay(50);
                }
                await notifService.ToastNotif("Success", "Sites master downloaded succesfully.");

            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
        }
        private async Task UpdateWarehouseMaster()
        {
            try
            {
                WarehouseList.Clear();
                var retdata = await serverDbWarehouseMasterService.GetList();
                WarehouseList.AddRange(retdata);
                decimal totcount = WarehouseLocationList.Count;
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
                    LoadingText = await percalc.GetPercentage("WarehouseLocation", decArray);
                    await Task.Delay(50);
                }
                await notifService.ToastNotif("Success", "Warehouse master downloaded succesfully.");

            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
        }
    }
}
#region 
/*private async Task UpdatePalletMaster()
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
            LoadingText = await percalc.GetPercentage("Users", decArray);
            await Task.Delay(50);
        }
        await notifService.ToastNotif("Success", "Pallet master downloaded succesfully.");
    }
    catch
    {
        await notifService.ToastNotif("Error", ErrorText);
    }
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
            var localDataCheck = await localDbWarehouseLocationMasterMasterService.GetFirstOrDefault(filter);
            if (localDataCheck == null)
            {
                await localDbWarehouseLocationMasterMasterService.Insert(item);
            }
            else
            {
                await localDbWarehouseLocationMasterMasterService.Update(item);
            }
            foreachcount++;
            decimal[] decArray = { foreachcount, totcount };
            LoadingText = await percalc.GetPercentage("WarehouseLocation", decArray);
            await Task.Delay(50);
        }
        await notifService.ToastNotif("Success", "WarehouseLocation master downloaded succesfully.");

    }
    catch
    {
        await notifService.StaticToastNotif("Error", ErrorText);
    }
}*/
#endregion