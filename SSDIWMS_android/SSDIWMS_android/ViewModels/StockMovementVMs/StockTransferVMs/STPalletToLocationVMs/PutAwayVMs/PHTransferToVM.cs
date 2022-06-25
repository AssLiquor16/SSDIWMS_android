using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.PutAwayVMs.PHTransferToPupSubVMs;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.PutAwayPages.PHTransferToPupSubPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.PutAwayVMs
{
    [QueryProperty(nameof(PalletCode), nameof(PalletCode))]
    [QueryProperty(nameof(WarehouseLoc), nameof(WarehouseLoc))]
    public class PHTransferToVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        GroupStockTransferHistory groupstockTransferHistory = new GroupStockTransferHistory();
        GlobalDependencyServices dependencies = new GlobalDependencyServices();

        WarehouseLocationModel _passedWarehouseLoc, _newWhLoc;
        string _palletCode,_warehouseLoc, _newWarehouseLoc,_newArea;

        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }
        public string WarehouseLoc { get => _warehouseLoc; set => SetProperty(ref _warehouseLoc, value); }
        public string NewWarehouseLoc { get => _newWarehouseLoc; set => SetProperty(ref _newWarehouseLoc, value); }
        public string NewArea { get => _newArea; set => SetProperty(ref _newArea, value); }

        
        public WarehouseLocationModel PassedWarehouseLoc { get => _passedWarehouseLoc; set => SetProperty(ref _passedWarehouseLoc, value); }
        public WarehouseLocationModel NewWhLoc { get => _newWhLoc; set => SetProperty(ref _newWhLoc, value); }

        public ObservableRangeCollection<PalletDetailsModel> PalletDetailsList { get; set; }

        public AsyncCommand SaveNewLocationCommand { get; }
        public AsyncCommand WarehouseLocationNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PHTransferToVM()
        {
            PalletDetailsList = new ObservableRangeCollection<PalletDetailsModel>();

            SaveNewLocationCommand = new AsyncCommand(SaveNewLocation);
            WarehouseLocationNavCommand = new AsyncCommand(WarehouseLocationNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

            MessagingCenter.Subscribe<WhLocListPupVM, WarehouseLocationModel>(this, "SetNewWarehouseLocation", async (sender, e) =>
             {
                 NewWhLoc = e;
                 NewWarehouseLoc = e.Final_Location;
                 NewArea = e.Area;
                 await Task.Delay(100);
             });
        }
        private async Task SaveNewLocation()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Saving...");

            if (!string.IsNullOrWhiteSpace(NewWarehouseLoc) && !string.IsNullOrWhiteSpace(NewArea))
            {
                try
                {
                    var serverPalleths = await dependencies.serverDbPalletHeaderService.GetList(new PalletHeaderModel { PalletCode = PalletCode }, "PalletCode");
                    if (serverPalleths.Count() == 1)
                    {
                        var transdate = DateTime.Now;
                        var mobileSerial = dependencies.droidService.GetDeviceInfo("Serial").ToUpper();
                        var oldwhloc = WarehouseLoc;
                        var newwhloc = NewWarehouseLoc;
                        var warehouseName = Preferences.Get("PrefWarehouseName", string.Empty);
                        foreach (var serverPalleth in serverPalleths)
                        {
                            List<StockTransferHistoryModel> stkTfHtry = new List<StockTransferHistoryModel>();
                            serverPalleth.WarehouseLocation = NewWarehouseLoc;
                            serverPalleth.DateSync = DateTime.Now;
                            serverPalleth.TimesUpdated++;
                            serverPalleth.Area = NewWhLoc.Area;
                            await dependencies.serverDbPalletHeaderService.Update(serverPalleth);
                            foreach (var serverPalletd in PalletDetailsList)
                            {

                                var oldstckflter = new StockCardsModel
                                {
                                    Warehouse_Location = oldwhloc,
                                    ItemCode = serverPalletd.ItemCode,
                                };
                                var oldstckcrditms = await dependencies.serverDbStockCardService.GetList(oldstckflter, "ItemCode/WarehouseLocation");
                                foreach (var oldstckcrditm in oldstckcrditms)
                                {
                                    oldstckcrditm.OnStock -= serverPalletd.Qty;
                                    await dependencies.serverDbStockCardService.Update(oldstckcrditm);
                                }
                                var newstckflter = new StockCardsModel
                                {
                                    Warehouse_Location = newwhloc,
                                    ItemCode = serverPalletd.ItemCode
                                };
                                var newstckcrditms = await dependencies.serverDbStockCardService.GetList(newstckflter, "ItemCode/WarehouseLocation");
                                if (newstckcrditms.Count() == 0)
                                {
                                    var imstrfltr = new ItemMasterModel
                                    {
                                        ItemCode = serverPalletd.ItemCode,
                                    };
                                    var imstr = await dependencies.localDbArticleMasterService.GetFirstOrDefault(imstrfltr, "ItemCode");
                                    var icode = serverPalletd.ItemCode;
                                    var nonexiststckcrd = new StockCardsModel
                                    {
                                        ItemCode = serverPalletd.ItemCode,
                                        ItemDesc = serverPalletd.ItemDesc,
                                        OnStock = serverPalletd.Qty,
                                        OnCommited = 0,
                                        OnBegginning = 0,
                                        ItemCategory1 = imstr.Category,
                                        ItemCategory2 = imstr.Division,
                                        Brand = imstr.Brand,
                                        Warehouse_Location = newwhloc,
                                        DateCreated = DateTime.Now,
                                        TimesUpdated = 0,
                                        DateSync = DateTime.Now,
                                        StockCardLocalId = 0,
                                        Area = NewArea,
                                        Warehouse = warehouseName
                                    };
                                    await dependencies.serverDbStockCardService.Insert(nonexiststckcrd);

                                    var newvl = new StockTransferHistoryModel
                                    {
                                        MobileId = mobileSerial,
                                        ItemCode = serverPalletd.ItemCode,
                                        ItemDesc = serverPalletd.ItemDesc,
                                        PalletCode = serverPalletd.PalletCode,
                                        TransferType = "Pallet To Location",
                                        TransactionType = "Put Away",
                                        FromLocation = oldwhloc,
                                        ToLocation = newwhloc,
                                        DateTransact = transdate,
                                        UserId = Preferences.Get("PrefUserId", 0),
                                        TimesUpdated = 0,
                                        DateSync = DateTime.Now,
                                        StockTransferLocalId = RandomStringGenerator.RandomString("STH"),
                                        Area = NewArea,
                                        Warehouse = warehouseName,
                                        Qty = serverPalletd.Qty,
                                    };
                                    stkTfHtry.Add(newvl);
                                }
                                else if (newstckcrditms.Count() == 1)
                                {
                                    foreach (var newstckcrditm in newstckcrditms)
                                    {
                                        newstckcrditm.OnStock += serverPalletd.Qty;
                                        newstckcrditm.Warehouse_Location = newwhloc;
                                        newstckcrditm.TimesUpdated++;
                                        newstckcrditm.Area = NewArea;
                                        newstckcrditm.DateSync = DateTime.Now;
                                        var e = await dependencies.serverDbStockCardService.Update(newstckcrditm);

                                        var newvl = new StockTransferHistoryModel
                                        {
                                            MobileId = mobileSerial,
                                            ItemCode = serverPalletd.ItemCode,
                                            ItemDesc = serverPalletd.ItemDesc,
                                            PalletCode = serverPalletd.PalletCode,
                                            TransferType = "Pallet To Location",
                                            TransactionType = "Put Away",
                                            FromLocation = oldwhloc,
                                            ToLocation = newwhloc,
                                            DateTransact = transdate,
                                            UserId = Preferences.Get("PrefUserId", 0),
                                            TimesUpdated = 0,
                                            DateSync = DateTime.Now,
                                            StockTransferLocalId = RandomStringGenerator.RandomString("STH"),
                                            Area = NewArea,
                                            Warehouse = warehouseName,
                                            Qty = serverPalletd.Qty,
                                        };
                                        stkTfHtry.Add(newvl);
                                    }
                                }
                            }
                            var groupestckthtry = await groupstockTransferHistory.GroupStockCard(stkTfHtry);
                            foreach (var groupe in groupestckthtry)
                            {
                                await dependencies.serverDbStockTransferHistoriesService.Insert(groupe);
                            }
                        }
                        PassedWarehouseLoc.MaxPallet--;
                        NewWhLoc.MaxPallet++;
                        await dependencies.serverDbTWarehouseLocationService.Update(PassedWarehouseLoc);
                        await dependencies.serverDbTWarehouseLocationService.Update(NewWhLoc);
                        await Shell.Current.GoToAsync($"..");

                    }
                }
                catch { await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server."); }
            } else { await dependencies.notifService.StaticToastNotif("Error", "Missing Entry."); }
            
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task WarehouseLocationNav()
        {
            await PopupNavigation.Instance.PushAsync(new WhLocListPupPage());
        }
        private async Task PageRefresh()
        {
            if (Preferences.Get("PHTransferToInitialRefresh", false) == false)
            {
                await dependencies.notifService.LoadingProcess("Begin", "Loading...");
                try
                {
                    PalletDetailsList.Clear();
                    await livetime.LiveTimer();
                    var whlocList = dependencies.serverDbTWarehouseLocationService.GetList(new WarehouseLocationModel { Final_Location = WarehouseLoc }, "Final_Loc");
                    await Task.Delay(100);
                    var pdetails = await dependencies.serverDbPalletDetailsService.GetList(new PalletDetailsModel { PalletCode = PalletCode }, "PalletCode");
                    await Task.Delay(100);
                    var whlocs = whlocList.Result.ToList();
                    PassedWarehouseLoc = whlocs.FirstOrDefault();
                    PalletDetailsList.AddRange(pdetails);
                    await Task.Delay(200);
                }
                catch
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Cannot connect to server, please try again?", "Ok");
                    await Shell.Current.GoToAsync($"..");
                }
                await dependencies.notifService.LoadingProcess("End");
                Preferences.Set("PHTransferToInitialRefresh", true);
            }
            
        }
    }
}
