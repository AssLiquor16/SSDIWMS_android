using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.StockMovementPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.StockMovementVMs
{
    public class SMPHTransferFromVM : ViewModelBase
    {
        public LiveTime liveTime { get; } = new LiveTime();
        GlobalDependencyServices dependencies = new GlobalDependencyServices();

        PalletHeaderModel _selectedItem;
        string _searchCode;

        public string SearchCode { get => _searchCode; set => SetProperty(ref _searchCode, value); }
        public PalletHeaderModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public ObservableRangeCollection<PalletHeaderModel> PalletHeaderList { get; set; }
        public AsyncCommand ApiSearchCommand { get; }
        public SMPHTransferFromVM()
        {
            PalletHeaderList = new ObservableRangeCollection<PalletHeaderModel>();
            TappedCommand = new AsyncCommand(Tapped);
            ApiSearchCommand = new AsyncCommand(ApiSearch);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Tapped()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Processing...");
            if (SelectedItem != null)
            {
                var isTransfer = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to transfer this item?", "Yes", "No");
                if (isTransfer == true)
                {
                    Preferences.Set("PrefSelectedPH.CurrentWhLoc", SelectedItem.WarehouseLocation);
                    Preferences.Remove("SMPHTransferToInitialRefresh");
                    await Shell.Current.GoToAsync($"{nameof(SMPHTransferToPage)}?PalletCode={SelectedItem.PalletCode}&WarehouseLoc={SelectedItem.WarehouseLocation}");
                    SelectedItem = null;
                }
                else { }
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        public async Task ApiSearch()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Searching...");
            if (!string.IsNullOrWhiteSpace(SearchCode))
            {
                var val = SearchCode.ToUpperInvariant();
                try
                {
                    var e = await dependencies.serverDbPalletHeaderService.GetList(new PalletHeaderModel { PalletCode = val }, "PalletCode");
                    PalletHeaderList.ReplaceRange(e.Where(x=>x.Area != "STAGE").ToList());
                }
                catch{ await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server"); }
                
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task PageRefresh()
        {
            await Task.Delay(10);
            await liveTime.LiveTimer();
            PalletHeaderList.Clear();
        }
    }
}
