using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.PutAwayPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.PutAwayVMs
{
    public class PHTransferFromVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        GlobalDependencyServices dependencies = new GlobalDependencyServices();

        PalletHeaderModel _selectedItem;
        string _searchCode;

        public string SearchCode { get => _searchCode; set => SetProperty(ref _searchCode, value); }
        public PalletHeaderModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public ObservableRangeCollection<PalletDetailsModel> PalletDetailList { get; set; }
        public ObservableRangeCollection<PalletHeaderModel> PalletHeaderList { get; set; }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ApiSearchCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PHTransferFromVM()
        {
            TappedCommand = new AsyncCommand(Tapped);
            PalletHeaderList = new ObservableRangeCollection<PalletHeaderModel>();
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
                    Preferences.Remove("PHTransferToInitialRefresh");
                    await Shell.Current.GoToAsync($"{nameof(PHTransferToPage)}?PalletCode={SelectedItem.PalletCode}&WarehouseLoc={SelectedItem.WarehouseLocation}");
                           SelectedItem = null;
                    }
                    else { }
                await dependencies.notifService.LoadingProcess("End");
            }
        }
        public async Task ApiSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchCode))
            {
                await dependencies.notifService.LoadingProcess("Begin", "Searching...");
                try
                {
                    var val = SearchCode.ToUpperInvariant();
                    var pheaders = await dependencies.serverDbPalletHeaderService.GetList(new PalletHeaderModel { PalletCode = val }, "PalletCode");
                    var phead = pheaders.Where(x => x.WarehouseLocation.Contains("STAGE")).ToList();
                    PalletHeaderList.ReplaceRange(phead);
                }
                catch
                {
                    await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
                await dependencies.notifService.LoadingProcess("End");
            }
        }
        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            PalletHeaderList.Clear();
        }
    }
}
