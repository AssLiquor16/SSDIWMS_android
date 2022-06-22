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

        #region Commands
        public AsyncCommand PalletListNavCommand { get; }
        public AsyncCommand WarehouseListNavCommand { get; }
        public AsyncCommand ItemListNavCommand { get; }
        public AsyncCommand SavePalletDetailsCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        #endregion

        public PalletAddVM()
        {
            PalletListNavCommand = new AsyncCommand(PalletListNav);
            WarehouseListNavCommand = new AsyncCommand(WarehouseListNav);
            ItemListNavCommand = new AsyncCommand(ItemListNav);
            SavePalletDetailsCommand = new AsyncCommand(SavePalletDetails);
            ToBeAddPalletDetailsList = new ObservableRangeCollection<ItemWithQtyModel>();
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

            #region Subscribe Messages from this.SubViewModels
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
            #endregion
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

        #region Navigations To Sub Pages/PopupPages
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
        #endregion

        #region Methods from ThisPage
        private void AddToList(ItemWithQtyModel obj)
        {
            var e = ToBeAddPalletDetailsList.Where(x => x.Item.ItemCode == obj.Item.ItemCode).FirstOrDefault();
            if(e == null)
            {
                ToBeAddPalletDetailsList.Add(obj);
            }
            else
            {
                if(e.ExpiryDate != obj.ExpiryDate)
                {
                    ToBeAddPalletDetailsList.Add(obj);
                }
                else
                {
                    int index = ToBeAddPalletDetailsList.IndexOf(e);
                    if (index != -1)
                    {
                        e.Qty += obj.Qty;
                        ToBeAddPalletDetailsList[index] = e;
                    }
                }
            }
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
        public static string RandomString(string firstchar)
        {
            int length = 3;
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rand = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return $"{firstchar}{rand}{DateTime.Now.ToString("MddyyHHmmssff")}";

        }
        #endregion

        #region Save = PalletHeader/PalletDetails/StockCard/StockTransferHistory/PalletMaster/WarehouseLocation
        private async Task SavePalletDetails()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Saving...");
            if (await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to insert this pallet header?", "Ok", "Cancel") == true)
            {
                if (!string.IsNullOrWhiteSpace(SelectedpalletMaster.PalletCode) && !string.IsNullOrWhiteSpace(SelectedWarehouseLocation.Final_Location))
                {
                    if (ToBeAddPalletDetailsList.Count > 0)
                    {
                        var transdate = DateTime.Now;
                        var deviceSerial = dependencies.droidService.GetDeviceInfo("Serial").ToUpperInvariant();
                        PalletHeaderModel palletHeaderData = new PalletHeaderModel 
                        {
                            PalletCode = SelectedpalletMaster.PalletCode,
                            WarehouseLocation = SelectedWarehouseLocation.Final_Location,
                            UserId = Preferences.Get("PrefUserId", 0),
                            DateCreated = transdate,
                            TimesUpdated = 0,
                            DateSync = DateTime.Now,
                            PHeaderRefID = RandomString("PH"),
                            Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)

                        };
                        try
                        {
                            var sPalletHeader = await dependencies.serverDbPalletHeaderService.GetList(palletHeaderData, "PalletCode");
                            #region Pallet Header does not exist
                            if (sPalletHeader.Count() == 0)
                            {  
                                var pheaderId = 0;
                                var retsPHeader = await dependencies.serverDbPalletHeaderService.Insert(palletHeaderData);
                                pheaderId = retsPHeader.PHeadId;
                                List<StockCardsModel> returnedstockdatas = new List<StockCardsModel>();
                                foreach (var initPalletDetail in ToBeAddPalletDetailsList)
                                    {
                                        PalletDetailsModel palletDetailsData = new PalletDetailsModel
                                        {
                                            PHeadId = pheaderId,
                                            PalletCode = palletHeaderData.PalletCode,
                                            ItemCode = initPalletDetail.Item.ItemCode,
                                            ItemDesc = initPalletDetail.Item.ItemDesc,
                                            Qty = initPalletDetail.Qty,
                                            DateCreated = DateTime.Now,
                                            TimesUpdated = 0,
                                            DateSync = DateTime.Now,
                                            ExpiryDate = initPalletDetail.ExpiryDate,
                                            PDetRefId = RandomString("PD")
                                        };
                                        var retsPDetail = await dependencies.serverDbPalletDetailsService.Insert(palletDetailsData);

                                            var sCardFilter = new StockCardsModel
                                            {
                                                ItemCode = palletDetailsData.ItemCode,
                                                Warehouse_Location = SelectedWarehouseLocation.Final_Location
                                            };
                                            var retSCard = await dependencies.serverDbStockCardService.GetList(sCardFilter, "ItemCode/WarehouseLocation");
                                            if (retSCard.Count() == 0)
                                            {
                                                var retsStockCard = await dependencies.serverDbStockCardService.Insert(new StockCardsModel
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
                                                    Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)
                                                });
                                                returnedstockdatas.Add(retsStockCard);
                                                    #region
                                        /*await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                        {
                                            MobileId = deviceSerial,
                                            ItemCode = retsStockCard.ItemCode,
                                            ItemDesc = retsStockCard.ItemDesc,
                                            PalletCode = retsPDetail.PalletCode,
                                            TransferType = "Pallet to Location",
                                            TransactionType = "New Stocks",
                                            FromLocation = SelectedWarehouseLocation.Final_Location,
                                            ToLocation = SelectedWarehouseLocation.Final_Location,
                                            DateTransact = DateTime.Now,
                                            UserId = Preferences.Get("PrefUserId", 0),
                                            TimesUpdated = 0,
                                            DateSync = DateTime.Now,
                                            StockTransferLocalId = RandomString("STH"),
                                            Area = SelectedWarehouseLocation.Area,
                                            Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)
                                        }) ;*/
                                        #endregion

                                            }
                                            else if (retSCard.Count() == 1)
                                            {
                                                  foreach (var scard in retSCard)
                                                  {
                                                      scard.OnStock += retsPDetail.Qty; 
                                                      await dependencies.serverDbStockCardService.Update(scard);
                                                      returnedstockdatas.Add(scard);
                                                    #region
                                            /*await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                            {
                                                MobileId = deviceSerial,
                                                ItemCode = scard.ItemCode,
                                                ItemDesc = scard.ItemDesc,
                                                PalletCode = retsPDetail.PalletCode,
                                                TransferType = "Pallet to Location",
                                                TransactionType = "New Stocks",
                                                FromLocation = SelectedWarehouseLocation.Final_Location,
                                                ToLocation = SelectedWarehouseLocation.Final_Location,
                                                DateTransact = DateTime.Now,
                                                UserId = Preferences.Get("PrefUserId", 0),
                                                TimesUpdated = 0,
                                                DateSync = DateTime.Now,
                                                StockTransferLocalId = RandomString("STH"),
                                                Area = SelectedWarehouseLocation.Area,
                                                Warehouse = Preferences.Get("PrefWarehouseName", string.Empty),

                                            });*/
                                            #endregion
                                                  }
                                            }
                                    }
                                var stckHistories = await GroupStockCard(returnedstockdatas);
                                foreach(var stckHistory in stckHistories)
                                {
                                    await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                    {
                                        MobileId = deviceSerial,
                                        ItemCode = stckHistory.ItemCode,
                                        ItemDesc = stckHistory.ItemDesc,
                                        PalletCode = palletHeaderData.PalletCode,
                                        TransferType = "Pallet to Location",
                                        TransactionType = "New Stocks",
                                        FromLocation = SelectedWarehouseLocation.Final_Location,
                                        ToLocation = SelectedWarehouseLocation.Final_Location,
                                        DateTransact = DateTime.Now,
                                        UserId = Preferences.Get("PrefUserId", 0),
                                        TimesUpdated = 0,
                                        DateSync = DateTime.Now,
                                        StockTransferLocalId = RandomString("STH"),
                                        Area = SelectedWarehouseLocation.Area,
                                        Warehouse = Preferences.Get("PrefWarehouseName", string.Empty),

                                    });
                                }
                            }
                            #endregion

                            #region Pallet Header exist
                            else if (sPalletHeader.Count() == 1)
                            {
                                List<StockCardsModel> returnedstockdatas = new List<StockCardsModel>();
                                foreach (var sPhead in sPalletHeader)
                                {
                                   sPhead.WarehouseLocation = SelectedWarehouseLocation.Final_Location;
                                   sPhead.Warehouse = Preferences.Get("PrefWarehouseName", string.Empty);
                                   await dependencies.serverDbPalletHeaderService.Update(sPhead);
                                   foreach (var initPalletDetail in ToBeAddPalletDetailsList)
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
                                            palletDetailsData.PDetRefId = RandomString("PD");

                                            var i = new PalletDetailsModel
                                            {
                                                PalletCode = sPhead.PalletCode,
                                                ItemCode = initPalletDetail.Item.ItemCode
                                            };
                                            var spdetails = await dependencies.serverDbPalletDetailsService.GetList(i, "PalletCode/ItemCode");
                                            if (spdetails.Count() == 0)
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
                                                    var e = await dependencies.serverDbStockCardService.Insert(new StockCardsModel
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
                                                        Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)
                                                    });
                                                    returnedstockdatas.Add(e);
                                                    #region
                                                    /*await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
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
                                                        StockTransferLocalId = RandomString("STH"),
                                                        Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                        Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)
                                                    });*/
                                                    #endregion
                                                }
                                                else if (retSCard.Count() == 1)
                                                {
                                                    foreach (var scard in retSCard)
                                                    {
                                                        scard.OnStock += retPdDetail.Qty;
                                                        await dependencies.serverDbStockCardService.Update(scard);
                                                        returnedstockdatas.Add(scard);
                                                        #region
                                                        /*await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
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
                                                            StockTransferLocalId = RandomString("STH"),
                                                            Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                            Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)
                                                        });*/
                                                        #endregion
                                                    }
                                                }
                                            }
                                            else if (spdetails.Count() == 1)
                                            {
                                                foreach (var sp in spdetails)
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
                                                        var e = await dependencies.serverDbStockCardService.Insert(new StockCardsModel
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
                                                        returnedstockdatas.Add(e);
                                                        #region
                                                        /*await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
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
                                                            StockTransferLocalId = RandomString("STH"),
                                                            Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                            Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)
                                                        });*/
                                                        #endregion
                                                    }
                                                    else if (retSCard.Count() == 1)
                                                    {
                                                        foreach (var scard in retSCard)
                                                        {
                                                            scard.OnStock += sp.Qty;
                                                            await dependencies.serverDbStockCardService.Update(scard);
                                                            returnedstockdatas.Add(scard);
                                                            #region
                                                            /*await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
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
                                                                StockTransferLocalId = RandomString("STH"),
                                                                Area = Preferences.Get("PrefWarehouseName", string.Empty),
                                                                Warehouse = Preferences.Get("PrefWarehouseName", string.Empty)
                                                            });*/
                                                            #endregion
                                                        }
                                                    }
                                                }
                                            }


                                        }
                                }
                                var groupedsth = await GroupStockCard(returnedstockdatas);
                                foreach (var stckHistory in groupedsth)
                                {
                                    await dependencies.serverDbStockTransferHistoriesService.Insert(new StockTransferHistoryModel
                                    {
                                        MobileId = deviceSerial,
                                        ItemCode = stckHistory.ItemCode,
                                        ItemDesc = stckHistory.ItemDesc,
                                        PalletCode = palletHeaderData.PalletCode,
                                        TransferType = "Pallet to Location",
                                        TransactionType = "New Stocks",
                                        FromLocation = SelectedWarehouseLocation.Final_Location,
                                        ToLocation = SelectedWarehouseLocation.Final_Location,
                                        DateTransact = DateTime.Now,
                                        UserId = Preferences.Get("PrefUserId", 0),
                                        TimesUpdated = 0,
                                        DateSync = DateTime.Now,
                                        StockTransferLocalId = RandomString("STH"),
                                        Area = SelectedWarehouseLocation.Area,
                                        Warehouse = Preferences.Get("PrefWarehouseName", string.Empty),

                                    });
                                }
                            }
                            #endregion

                            await dependencies.notifService.StaticToastNotif("Success", "Pallet Header added succesfully.");


                        } catch { await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server."); }
                        await Task.Delay(300);
                        await Shell.Current.GoToAsync($"..");
                    }else{ await dependencies.notifService.StaticToastNotif("Error", "No item(s) added.");}
                }else{ await dependencies.notifService.StaticToastNotif("Error", "Missing  entry."); }
            }await dependencies.notifService.LoadingProcess("End");
        }
        private async Task<IEnumerable<StockCardsModel>>GroupStockCard(List<StockCardsModel> lists)
        {
            await Task.Delay(1);
            var e = lists.GroupBy(l=> l.ItemCode).Select(cl => new StockCardsModel
            {
                ItemCode = cl.Key,
                ItemDesc = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                OnStock = cl.Sum(x => x.OnStock),
                OnCommited = cl.Sum(x => x.OnStock),
                OnBegginning = cl.Sum(x => x.OnStock),
                ItemCategory1 = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemCategory1,
                ItemCategory2 = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemCategory2,
                Brand = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().Brand,
                DateCreated = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().DateCreated,
                DateSync = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().DateSync,
                StockCardLocalId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().StockCardLocalId,
                Area = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().Area,
                Warehouse = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().Warehouse,
            }).ToList();
            return e;
        }
        #endregion
    }
}
#region Comments
/*
 * SelectedpalletMaster.PalletStatus = "In-Use";
                                      await dependencies.serverDbTPalletMasterService.Update(SelectedpalletMaster);
                                      SelectedWarehouseLocation.MaxPallet++;
                                      await dependencies.serverDbTWarehouseLocationService.Update(SelectedWarehouseLocation);
 */
#endregion