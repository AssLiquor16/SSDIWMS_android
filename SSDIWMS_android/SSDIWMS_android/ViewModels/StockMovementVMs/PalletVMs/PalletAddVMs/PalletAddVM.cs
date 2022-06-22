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
        public static string RandomString()
        {
            int length = 3;
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task SavePalletDetails()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Saving...");
            if(await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to insert this pallet header?", "Ok", "Cancel") == true)
            {
            }
        }
    }
}


/*
 * SelectedpalletMaster.PalletStatus = "In-Use";
                                      await dependencies.serverDbTPalletMasterService.Update(SelectedpalletMaster);
                                      SelectedWarehouseLocation.MaxPallet++;
                                      await dependencies.serverDbTWarehouseLocationService.Update(SelectedWarehouseLocation);
 */