using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.TransactionModels;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster;
using SSDIWMS_android.Services.NotificationServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.PopUpVMs
{
    public class LoadingPopupVM : ViewModelBase
    {
        ILocalArticleMasterServices localDbArticleMasterService;
        ILocalPalletMasterServices localDbPalletmasterService;

        IServerArticleMasterServices serverDbArticleMasterService;
        IServerPalletMasterServices serverDbPalletMasterService;
        
        IToastNotifService notifService;

        string _staticloadingText,_loadingText, _taskType,_errorText;
        
        public ObservableRangeCollection<PalletMasterModel> PalletMasterList { get; set; }
        public ObservableRangeCollection<ArticleMasterModel> ArticleMasterList { get; set; }

        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }


        public AsyncCommand RefreshCommand { get; }
        public LoadingPopupVM()
        {
            localDbArticleMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            localDbPalletmasterService = DependencyService.Get<ILocalPalletMasterServices>();

             serverDbArticleMasterService = DependencyService.Get<IServerArticleMasterServices>();
            serverDbPalletMasterService = DependencyService.Get<IServerPalletMasterServices>();
           
            notifService = DependencyService.Get<IToastNotifService>();

            PalletMasterList = new ObservableRangeCollection<PalletMasterModel>();
            ArticleMasterList = new ObservableRangeCollection<ArticleMasterModel>();
            RefreshCommand = new AsyncCommand(Refresh);
        }
        public async Task Refresh()
        {
            ErrorText = "Cannot connect to server.";
            await Task.Delay(1);
            if (string.IsNullOrWhiteSpace(TaskType))
            {
                StaticLoadingText = "Processing...";
            }
            else if (TaskType == "Login")
            {
                StaticLoadingText = "Verifying credentials...";
            }
            else if (TaskType == "Logout")
            {
                StaticLoadingText = "Logging out...";
            }
            else if (TaskType.Contains("Update"))
            {
                StaticLoadingText = "Updating...";
                switch (TaskType)
                {
                    case "UpdateArticleMaster":
                        await UpdateArticleMaster();
                        break;
                    case "UpdatePalletMaster":
                        await UpdatePalletMaster();
                        break;
                    default:
                        break;
                }
                
            }

        }


        public async Task UpdateArticleMaster()
        {
            try
            {
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
                        await localDbArticleMasterService.Insert("Common",content);
                    }
                    else
                    {
                        await localDbArticleMasterService.Update("Common", content);
                    }
                    foreachcount++;
                    LoadingText = await PercentageCalculator(totcount, foreachcount);
                    await Task.Delay(50);
                }
                await notifService.StaticToastNotif("Success", "Article master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await CloseAll();
        }
        public async Task UpdatePalletMaster()
        {
            try
            {
                PalletMasterList.Clear();
                var retdata = await serverDbPalletMasterService.GetList("All", null, null);
                PalletMasterList.AddRange(retdata);
                decimal totcount = PalletMasterList.Count;
                decimal foreachcount = 0;
                foreach (var scontent in retdata)
                {
                    int[] intarray = { scontent.PId };
                    var ret = await localDbPalletmasterService.GetModel("PId", null, intarray);
                    var model = new PalletMasterModel
                    {
                        PId = scontent.PId,
                        PalletCode = scontent.PalletCode,
                        WarehouseId = scontent.WarehouseId,
                        DateCreated = scontent.DateCreated,
                        PalletStatus = scontent.PalletStatus,
                        PalletDescription = scontent.PalletDescription,
                    };
                    if(ret == null)
                    {
                        await localDbPalletmasterService.Insert("Common", model);
                    }
                    else
                    {
                        await localDbPalletmasterService.Update("Common", model);
                    }
                    foreachcount++;
                    LoadingText = await PercentageCalculator(totcount,foreachcount);
                    await Task.Delay(50);

                }
                await notifService.StaticToastNotif("Success", "Pallet master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await CloseAll();
        }
        

        public async Task<string> PercentageCalculator(decimal totalcount, decimal foreachcount)
        {
            await Task.Delay(1);
            decimal e = foreachcount / totalcount;
            decimal f = e * 100;
            decimal g = decimal.Round(f, 2, MidpointRounding.AwayFromZero);
            g = Math.Round(g, 0);
            var retstring = "";
            if(TaskType == "UpdateArticleMaster")
            {
                retstring = "Article master  " + g + "% out of 100%";
            }
            else if(TaskType == "UpdatePalletMaster")
            {
                retstring = "Pallet master  " + g + "% out of 100%";
            }
            return retstring;
        }
        public async Task CloseAll()
        {
            await PopupNavigation.Instance.PopAllAsync(true);
        }
        public async Task Close()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
