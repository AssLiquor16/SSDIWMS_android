using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.PutAwayPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.StockMovementVMs
{
    public class SMPHTrasnferFromVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        public LiveTime livetime { get; } = new LiveTime();
        string _searchCode;
        PalletHeaderModel _selectedItem;
        public ObservableRangeCollection<PalletHeaderModel> PalletHeaderList { get; set; }
        public PalletHeaderModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string SearchCode { get => _searchCode; set => SetProperty(ref _searchCode, value); }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ApiSearchCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public SMPHTrasnferFromVM()
        {
            PalletHeaderList = new ObservableRangeCollection<PalletHeaderModel>();
            TappedCommand = new AsyncCommand(Tapped);
            ApiSearchCommand = new AsyncCommand(ApiSearch);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Tapped()
        {
            if(SelectedItem != null)
            {
                await dependencies.notifService.LoadingProcess("Begin", "Processing...");
                var isTransfer = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to transfer this item?", "Yes", "No");
                if (isTransfer == true)
                {
                    Preferences.Set("PrefSelectedPH.CurrentWhLoc", SelectedItem.WarehouseLocation);
                    Preferences.Remove("SMPHTransferToInitialRefresh");
                    await Shell.Current.GoToAsync($"{nameof(PHTransferToPage)}?PalletCode={SelectedItem.PalletCode}&WarehouseLoc={SelectedItem.WarehouseLocation}");
                    SelectedItem = null;
                }
                else { }
                await dependencies.notifService.LoadingProcess("End");
                SelectedItem = null;
            }
        }
        private async Task ApiSearch()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Search...");
            if (!string.IsNullOrWhiteSpace(SearchCode))
            {
                try
                {
                    var val = SearchCode.ToUpperInvariant();
                    var palletheaders = await dependencies.serverDbPalletHeaderService.GetList(new PalletHeaderModel { PalletCode = val }, "PalletCode");
                    PalletHeaderList.ReplaceRange(palletheaders.Where(x => x.Warehouse == Preferences.Get("PrefWarehouseName", string.Empty) && x.Area != "STAGE").ToList());
                }
                catch
                {
                    await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server");
                }
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task PageRefresh()
        {
            PalletHeaderList.Clear();
            await livetime.LiveTimer();
        }
    }
}
