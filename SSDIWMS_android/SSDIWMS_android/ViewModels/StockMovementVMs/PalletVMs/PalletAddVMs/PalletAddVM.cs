using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using SSDIWMS_android.Models.SMTransactionModel.Temp;
using SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubPopupVMs;
using SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs;
using SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs.PalletAddItemListSubPopupVMs;
using SSDIWMS_android.Views.StockMovementPages.PalletPages.PalletAddPages.PalletAddSubPages;
using SSDIWMS_android.Views.StockMovementPages.PalletPages.PalletAddPages.PalletAddSubPopupPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs
{
    public class PalletAddVM : ViewModelBase
    {
        private static Random random = new Random();
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        ItemWithQtyModel _selectedItem;
        PalletMasterModel _selectedpalletMaster;
        WarehouseLocationModel _selectedWarehouseLocation;
        public PalletMasterModel SelectedpalletMaster { get => _selectedpalletMaster; set => SetProperty(ref _selectedpalletMaster, value); }
        public WarehouseLocationModel SelectedWarehouseLocation { get => _selectedWarehouseLocation; set => SetProperty(ref _selectedWarehouseLocation, value); }
        public ItemWithQtyModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public ObservableRangeCollection<ItemWithQtyModel> ToBeAddPalletDetailsList { get; set; }
        public AsyncCommand PalletListNavCommand { get; }
        public AsyncCommand WarehouseListNavCommand { get; }
        public AsyncCommand ItemListNavCommand { get; }
        public AsyncCommand SavePalletDetailsCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletAddVM()
        {
            PalletListNavCommand = new AsyncCommand(PalletListNav);
            WarehouseListNavCommand = new AsyncCommand(WarehouseListNav);
            ItemListNavCommand = new AsyncCommand(ItemListNav);
            SavePalletDetailsCommand = new AsyncCommand(SavePalletDetails);
            ToBeAddPalletDetailsList = new ObservableRangeCollection<ItemWithQtyModel>();
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

            MessagingCenter.Subscribe<PalletAddPalletListVM, PalletMasterModel>(this, "SetPallet", (sender, e) =>
              {
                  SelectedpalletMaster = e;
              });
            MessagingCenter.Subscribe<PalletAddWhLocListVM, WarehouseLocationModel>(this, "SetWarehouseLoc", (sender, e) =>
             {
                 SelectedWarehouseLocation = e;
             });
            MessagingCenter.Subscribe<PalletAddItemListSubPopupVM, ItemWithQtyModel>(this, "AddItem", (sender, e) =>
             {
                 AddToList(e);
             });
            MessagingCenter.Subscribe<PalletAddItemDetailPopupVM, ItemWithQtyModel>(this, "EditItem", (sender, e) =>
              {
                  EditTappedItem(e);
              });
            MessagingCenter.Subscribe<PalletAddItemDetailPopupVM, ItemWithQtyModel>(this, "RemoveItem", (sender, e) =>
            {
                RemoveItem(e);
            });
        }
        private async Task PalletListNav() => await Shell.Current.GoToAsync($"{nameof(PalletAddPalletListPage)}");
        private async Task WarehouseListNav() => await Shell.Current.GoToAsync($"{nameof(PalletAddWhLocListPage)}");
        private async Task ItemListNav() => await Shell.Current.GoToAsync($"{nameof(PalletAddItemListPage)}");
        private async Task Tapped()
        {
            if (SelectedItem != null)
            {
                await PopupNavigation.Instance.PushAsync(new PalletAddItemDetailPopupPage(SelectedItem));
            }
        }
        private async Task SavePalletDetails()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Saving...");
            if(await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to insert this pallet header?", "Ok", "Cancel") == true)
            {
                if (!string.IsNullOrWhiteSpace(SelectedpalletMaster.PalletCode) && !string.IsNullOrWhiteSpace(SelectedWarehouseLocation.Final_Location))
                {
                    if (ToBeAddPalletDetailsList.Count > 0)
                    {

                        PalletHeaderModel palletHeaderData = new PalletHeaderModel();
                        palletHeaderData.PalletCode = SelectedpalletMaster.PalletCode;
                        palletHeaderData.WarehouseLocation = SelectedWarehouseLocation.Final_Location;
                        palletHeaderData.UserId = Preferences.Get("PrefUserId", 0);
                        palletHeaderData.DateCreated = DateTime.Now;
                        palletHeaderData.TimesUpdated = 0;
                        palletHeaderData.DateSync = DateTime.Now;
                        palletHeaderData.PHeaderRefID = $"PH{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}";
                        palletHeaderData.Warehouse = Preferences.Get("PrefWarehouseName", string.Empty);
                        switch (Setup.PalletSaveMethod)
                        {
                            case "Online":
                                try
                                {
                                    var sPalletHeader = await dependencies.serverDbPalletHeaderService.GetList(palletHeaderData, "PalletCode");
                                    if (sPalletHeader.Count() == 0)
                                    {
                                        var pheaderId = 0;
                                        try
                                        {
                                            var retsPHeader = await dependencies.serverDbPalletHeaderService.Insert(palletHeaderData);
                                            pheaderId = retsPHeader.PHeadId;
                                        }
                                        catch
                                        {
                                            var retlPHeader = await dependencies.localDbPalletHeaderService.Insert(palletHeaderData);
                                            pheaderId = retlPHeader.PHeadId;
                                        }
                                        foreach (var initPalletDetail in ToBeAddPalletDetailsList)
                                        {
                                            PalletDetailsModel palletDetailsData = new PalletDetailsModel();
                                            palletDetailsData.PHeadId = pheaderId;
                                            palletDetailsData.PalletCode = palletHeaderData.PalletCode;
                                            palletDetailsData.ItemCode = initPalletDetail.Item.ItemCode;
                                            palletDetailsData.ItemDesc = initPalletDetail.Item.ItemDesc;
                                            palletDetailsData.Qty = initPalletDetail.Qty;
                                            palletDetailsData.DateCreated = DateTime.Now;
                                            palletDetailsData.TimesUpdated = 0;
                                            palletDetailsData.DateSync = DateTime.Now;
                                            palletDetailsData.ExpiryDate = initPalletDetail.ExpiryDate;
                                            palletDetailsData.PDetRefId = $"PD{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}";

                                            var retsPDetail = await dependencies.serverDbPalletDetailsService.Insert(palletDetailsData);
                                            
                                            try
                                            {
                                                var sCardFilter = new StockCardsModel
                                                {
                                                    ItemCode = palletDetailsData.ItemCode,
                                                    Warehouse_Location = SelectedWarehouseLocation.Final_Location
                                                };
                                                var retSCard = await dependencies.serverDbStockCardService.GetList(sCardFilter, "ItemCode/WarehouseLocation");
                                                if (retSCard.Count() == 0)
                                                {
                                                    await dependencies.serverDbStockCardService.Insert(new StockCardsModel
                                                    {
                                                        ItemCode = retsPDetail.ItemCode,
                                                        ItemDesc = retsPDetail.ItemDesc,
                                                        OnStock = retsPDetail.Qty,
                                                        OnCommited = 0,
                                                        OnBegginning = 0,
                                                        ItemCategory1 = initPalletDetail.Item.Category,
                                                        ItemCategory2 = initPalletDetail.Item.Division,
                                                        Brand = initPalletDetail.Item.Brand,
                                                        Warehouse_Location = SelectedWarehouseLocation.Final_Location,
                                                        DateCreated = DateTime.Now,
                                                        TimesUpdated = 0,
                                                        DateSync = DateTime.Now,
                                                        Area = SelectedWarehouseLocation.Area,
                                                        Warehouse = SelectedWarehouseLocation.Warehouse
                                                    });
                                                }
                                                else if (retSCard.Count() == 1)
                                                {
                                                    foreach (var scard in retSCard)
                                                    {
                                                        scard.OnStock += retsPDetail.Qty;
                                                        await dependencies.serverDbStockCardService.Update(scard);
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }

                                        }
                                    }
                                    else if(sPalletHeader.Count() == 1)
                                    {
                                      foreach(var sPhead in sPalletHeader)
                                      {
                                          try
                                          {
                                              sPhead.WarehouseLocation = SelectedWarehouseLocation.Final_Location;
                                              sPhead.Warehouse = Preferences.Get("PrefWarehouseName", string.Empty);
                                              await dependencies.serverDbPalletHeaderService.Update(sPhead);
                                          }
                                          catch
                                          {
                                                await dependencies.localDbPalletHeaderService.Update(sPhead);
                                          }
                                            foreach(var initPalletDetail in ToBeAddPalletDetailsList)
                                            {
                                                PalletDetailsModel palletDetailsData = new PalletDetailsModel();
                                                palletDetailsData.PHeadId = sPhead.PHeadId;
                                                palletDetailsData.PalletCode = palletHeaderData.PalletCode;
                                                palletDetailsData.ItemCode = initPalletDetail.Item.ItemCode;
                                                palletDetailsData.ItemDesc = initPalletDetail.Item.ItemDesc;
                                                palletDetailsData.Qty = initPalletDetail.Qty;
                                                palletDetailsData.DateCreated = DateTime.Now;
                                                palletDetailsData.TimesUpdated = 0;
                                                palletDetailsData.DateSync = DateTime.Now;
                                                palletDetailsData.ExpiryDate = initPalletDetail.ExpiryDate;
                                                palletDetailsData.PDetRefId = $"PD{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}";

                                                var i = new PalletDetailsModel
                                                {
                                                    PalletCode = sPhead.PalletCode,
                                                    ItemCode = initPalletDetail.Item.ItemCode
                                                };
                                                var spdetails = await dependencies.serverDbPalletDetailsService.GetList(i, "PalletCode/ItemCode");
                                                if(spdetails.Count() == 0)
                                                {
                                                    var retPdDetail = await dependencies.serverDbPalletDetailsService.Insert(palletDetailsData);
                                                    var sCardFilter = new StockCardsModel
                                                    {
                                                        ItemCode = palletDetailsData.ItemCode,
                                                        Warehouse_Location = SelectedWarehouseLocation.Final_Location
                                                    };
                                                    var retSCard = await dependencies.serverDbStockCardService.GetList(sCardFilter, "" +
                                                        "ItemCode/WarehouseLocation");
                                                    if (retSCard.Count() == 0)
                                                    {
                                                        await dependencies.serverDbStockCardService.Insert(new StockCardsModel
                                                        {
                                                            ItemCode = retPdDetail.ItemCode,
                                                            ItemDesc = retPdDetail.ItemDesc,
                                                            OnStock = retPdDetail.Qty,
                                                            OnCommited = 0,
                                                            OnBegginning = 0,
                                                            ItemCategory1 = initPalletDetail.Item.Category,
                                                            ItemCategory2 = initPalletDetail.Item.Division,
                                                            Brand = initPalletDetail.Item.Brand,
                                                            Warehouse_Location = SelectedWarehouseLocation.Final_Location,
                                                            DateCreated = DateTime.Now,
                                                            TimesUpdated = 0,
                                                            DateSync = DateTime.Now,
                                                            Area = SelectedWarehouseLocation.Area,
                                                            Warehouse = SelectedWarehouseLocation.Warehouse
                                                        });

                                                        await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                                        {
                                                            ItemCode = retPdDetail.ItemCode,
                                                            ItemDesc = retPdDetail.ItemDesc,
                                                            PalletCode = retPdDetail.PalletCode,
                                                            TransferType = "Pallet to Location",
                                                            TransactionType = "New-Stocks",
                                                            FromLocation = SelectedWarehouseLocation.Final_Location,
                                                            ToLocation = SelectedWarehouseLocation.Final_Location,
                                                            DateTransact = DateTime.Now,
                                                            UserId = Preferences.Get("PrefUserId", 0),
                                                            TimesUpdated = 0,
                                                            DateSync = DateTime.Now,
                                                            StockTransferLocalId = $"STH{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}",
                                                            Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                            Warehouse = SelectedWarehouseLocation.Warehouse
                                                        });
                                                    }
                                                    else if (retSCard.Count() == 1)
                                                    {
                                                        foreach (var scard in retSCard)
                                                        {
                                                            scard.OnStock += retPdDetail.Qty;
                                                            await dependencies.serverDbStockCardService.Update(scard);
                                                            await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                                            {
                                                                ItemCode = scard.ItemCode,
                                                                ItemDesc = scard.ItemDesc,
                                                                PalletCode = retPdDetail.PalletCode,
                                                                TransferType = "Pallet to Location",
                                                                TransactionType = "New-Stocks",
                                                                FromLocation = SelectedWarehouseLocation.Final_Location,
                                                                ToLocation = SelectedWarehouseLocation.Final_Location,
                                                                DateTransact = DateTime.Now,
                                                                UserId = Preferences.Get("PrefUserId", 0),
                                                                TimesUpdated = 0,
                                                                DateSync = DateTime.Now,
                                                                StockTransferLocalId = $"STH{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}",
                                                                Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                                Warehouse = SelectedWarehouseLocation.Warehouse
                                                            });
                                                        }
                                                    }
                                                }
                                                else if(spdetails.Count() == 1)
                                                {
                                                    foreach(var sp in spdetails)
                                                    {
                                                        sp.TimesUpdated++;
                                                        sp.Qty += palletDetailsData.Qty;
                                                        sp.DateSync = DateTime.Now;
                                                        sp.ExpiryDate = palletDetailsData.ExpiryDate;
                                                        await dependencies.serverDbPalletDetailsService.Update(sp);

                                                        var sCardFilter = new StockCardsModel
                                                        {
                                                            ItemCode = sp.ItemCode,
                                                            Warehouse_Location = SelectedWarehouseLocation.Final_Location
                                                        };
                                                        var retSCard = await dependencies.serverDbStockCardService.GetList(sCardFilter, "" +
                                                            "ItemCode/WarehouseLocation");
                                                        if (retSCard.Count() == 0)
                                                        {
                                                            await dependencies.serverDbStockCardService.Insert(new StockCardsModel
                                                            {
                                                                ItemCode = sp.ItemCode,
                                                                ItemDesc = sp.ItemDesc,
                                                                OnStock = sp.Qty,
                                                                OnCommited = 0,
                                                                OnBegginning = 0,
                                                                ItemCategory1 = initPalletDetail.Item.Category,
                                                                ItemCategory2 = initPalletDetail.Item.Division,
                                                                Brand = initPalletDetail.Item.Brand,
                                                                Warehouse_Location = SelectedWarehouseLocation.Final_Location,
                                                                DateCreated = DateTime.Now,
                                                                TimesUpdated = 0,
                                                                DateSync = DateTime.Now,
                                                                Area = SelectedWarehouseLocation.Area,
                                                                Warehouse = SelectedWarehouseLocation.Warehouse
                                                            });

                                                            await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                                            {
                                                                ItemCode = sp.ItemCode,
                                                                ItemDesc = sp.ItemDesc,
                                                                PalletCode = sp.PalletCode,
                                                                TransferType = "Pallet to Location",
                                                                TransactionType = "New-Stocks",
                                                                FromLocation = SelectedWarehouseLocation.Final_Location,
                                                                ToLocation = SelectedWarehouseLocation.Final_Location,
                                                                DateTransact = DateTime.Now,
                                                                UserId = Preferences.Get("PrefUserId", 0),
                                                                TimesUpdated = 0,
                                                                DateSync = DateTime.Now,
                                                                StockTransferLocalId = $"STH{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}",
                                                                Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                                Warehouse = SelectedWarehouseLocation.Warehouse
                                                            });
                                                        }
                                                        else if (retSCard.Count() == 1)
                                                        {
                                                            foreach (var scard in retSCard)
                                                            {
                                                                scard.OnStock += sp.Qty;
                                                                await dependencies.serverDbStockCardService.Update(scard);

                                                                await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                                                {
                                                                    ItemCode = scard.ItemCode,
                                                                    ItemDesc = scard.ItemDesc,
                                                                    PalletCode = sp.PalletCode,
                                                                    TransferType = "Pallet to Location",
                                                                    TransactionType = "New-Stocks",
                                                                    FromLocation = SelectedWarehouseLocation.Final_Location,
                                                                    ToLocation = SelectedWarehouseLocation.Final_Location,
                                                                    DateTransact = DateTime.Now,
                                                                    UserId = Preferences.Get("PrefUserId", 0),
                                                                    TimesUpdated = 0,
                                                                    DateSync = DateTime.Now,
                                                                    StockTransferLocalId = $"STH{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}",
                                                                    Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                                    Warehouse = SelectedWarehouseLocation.Warehouse
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                                

                                            }
                                        }
                                    }
                                    await dependencies.notifService.StaticToastNotif("Success", "Pallet Header added succesfully.");
                                }
                                catch
                                {
                                    await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server.");
                                }
                                break;
                            case "Offline":
                                var pallet = await dependencies.localDbPalletHeaderService.GetFirstOrDefault(palletHeaderData, "PalletCode");
                                if (pallet == null)
                                {
                                    await dependencies.localDbPalletHeaderService.Insert(palletHeaderData);
                                    foreach (var initialPalletdetail in ToBeAddPalletDetailsList)
                                    {
                                        PalletDetailsModel palletDetailsData = new PalletDetailsModel();
                                        palletDetailsData.PalletCode = palletHeaderData.PalletCode;
                                        palletDetailsData.ItemCode = initialPalletdetail.Item.ItemCode;
                                        palletDetailsData.ItemDesc = initialPalletdetail.Item.ItemDesc;
                                        palletDetailsData.Qty = initialPalletdetail.Qty;
                                        palletDetailsData.DateCreated = DateTime.Now;
                                        palletDetailsData.TimesUpdated = 0;
                                        palletDetailsData.DateSync = DateTime.Now;
                                        palletDetailsData.PDetRefId = $"PD{RandomString()}{DateTime.Now.ToString("MddyyHHmmssff")}";
                                        await dependencies.localDbPalletDetailsService.Insert(palletDetailsData);
                                    }
                                }
                                else
                                {
                                    await dependencies.notifService.StaticToastNotif("Error", "Data already exist.");
                                }
                                break;
                            default: break;
                        }
                        await Task.Delay(300);
                        await Shell.Current.GoToAsync($"..");
                    }
                    else
                    {
                        await dependencies.notifService.StaticToastNotif("Error", "No item(s) added.");
                    }
                }
                else
                {
                    await dependencies.notifService.StaticToastNotif("Error", "Missing  entry.");
                }
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        public static string RandomString()
        {
            int length = 3;
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        private void AddToList(ItemWithQtyModel obj)
        {
            ToBeAddPalletDetailsList.Add(obj);
        }
        private void EditTappedItem(ItemWithQtyModel obj)
        {
            int index = ToBeAddPalletDetailsList.IndexOf(SelectedItem);
            if (index != -1)
            {
                ToBeAddPalletDetailsList[index] = obj;
            }
            SelectedItem = null;
        }
        private void RemoveItem(ItemWithQtyModel obj)
        {
            ToBeAddPalletDetailsList.Remove(SelectedItem);
        }

        private async Task PageRefresh()
        {
            await Task.Delay(100);
            await dependencies.notifService.LoadingProcess("Begin", "Loading...");
            try
            {
                if (Preferences.Get("PrefPalletAddPageInitialRefresh", false) == false)
                {
                    var stagingloc = await dependencies.localDbStagingWarehouseLocationService.GetFirstOrDefault();
                    SelectedWarehouseLocation = new WarehouseLocationModel
                    {
                        LocId = stagingloc.LocId,
                        Warehouse = stagingloc.Warehouse,
                        Area = stagingloc.Area,
                        Rack = stagingloc.Rack,
                        Level = stagingloc.Level,
                        Bin = stagingloc.Bin,
                        UOM = stagingloc.UOM,
                        Final_Location = stagingloc.Final_Location,
                        DateCreated = stagingloc.DateCreated,
                        DateUpdated = stagingloc.DateUpdated,
                        MultiplePallet = stagingloc.MultiplePallet,
                        IsBlockStock = stagingloc.IsBlockStock,
                        MaxPallet = stagingloc.MaxPallet
                    };
                    ToBeAddPalletDetailsList.Clear();
                    Preferences.Set("PrefPalletAddPageInitialRefresh", true);
                }
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Cannot query the location, please try again.", "Ok");
                await Shell.Current.GoToAsync($"..");
            }
            await dependencies.notifService.LoadingProcess("End");
        }
    }
}


/*
 * SelectedpalletMaster.PalletStatus = "In-Use";
                                      await dependencies.serverDbTPalletMasterService.Update(SelectedpalletMaster);
                                      SelectedWarehouseLocation.MaxPallet++;
                                      await dependencies.serverDbTWarehouseLocationService.Update(SelectedWarehouseLocation);
 */