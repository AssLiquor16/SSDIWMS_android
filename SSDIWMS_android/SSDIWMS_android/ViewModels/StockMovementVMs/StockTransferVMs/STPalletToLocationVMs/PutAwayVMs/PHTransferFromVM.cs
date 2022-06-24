using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.PutAwayPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
                var isTransfer = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to transfer this item?", "Yes", "No");
                if (isTransfer == true)
                {
                    await Shell.Current.GoToAsync($"{nameof(PHTransferToPage)}");
                    SelectedItem = null;
                } else { }
               
            }
        }
        private async Task ApiSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchCode))
            {
                await dependencies.notifService.LoadingProcess("Begin", "Searching...");
                try
                {
                    var val = SearchCode.ToUpperInvariant();
                    var pheaders = await dependencies.serverDbPalletHeaderService.GetList(new PalletHeaderModel { PalletCode = val }, "PalletCode");
                    PalletHeaderList.ReplaceRange(pheaders);
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
