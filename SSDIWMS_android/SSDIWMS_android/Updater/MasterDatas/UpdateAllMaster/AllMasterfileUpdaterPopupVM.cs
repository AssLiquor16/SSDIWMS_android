﻿using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
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
        IMainServices mainServices;
        ILocalArticleMasterServices localDbArticleMasterService;
        IServerArticleMasterServices serverDbArticleMasterService;
        ILocalPalletMasterServices localDbPalletMasterService;
        IServerPalletMasterServices serverDbPalletMasterService;
        IToastNotifService notifService;

        string _staticloadingText, _loadingText, _taskType, _errorText;

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public ObservableRangeCollection<ArticleMasterModel> ArticleMasterList { get; set; }
        public ObservableRangeCollection<PalletMasterModel> PalletMasterList { get; set; }

        public AsyncCommand PageRefreshCommand { get; }
        
        public AllMasterfileUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbArticleMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            serverDbArticleMasterService = DependencyService.Get<IServerArticleMasterServices>();
            localDbPalletMasterService = DependencyService.Get<ILocalPalletMasterServices>();
            serverDbPalletMasterService = DependencyService.Get<IServerPalletMasterServices>();
            notifService = DependencyService.Get<IToastNotifService>();

            ArticleMasterList = new ObservableRangeCollection<ArticleMasterModel>();
            PalletMasterList = new ObservableRangeCollection<PalletMasterModel>();

            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        public async Task PageRefresh()
        {
            StaticLoadingText = "Processing...";
            await UpdateArticleMaster();
            await UpdatePalletMaster();
            await PopupNavigation.Instance.PopAsync(true);
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
                    LoadingText = await mainServices.GetPercentage("ArticleMaster", decArray);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "Article master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
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
                    LoadingText = await mainServices.GetPercentage("PalletMaster", decArray);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "Pallet master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
        }
    }
}