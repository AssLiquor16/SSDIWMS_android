using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Views.StockMovementPages.PalletPages;
using SSDIWMS_android.Views.StockMovementPages.PalletPages.PalletAddPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs
{
    public class PalletHeaderListVM : ViewModelBase
    {
        string _apiSearchCode;
        PalletHeaderModel _selectedItem;
        public LiveTime livetime { get; } = new LiveTime();
        public GlobalDependencyServices dependencies = new GlobalDependencyServices();
        public PalletHeaderModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string ApiSearchCode { get => _apiSearchCode; set => SetProperty(ref _apiSearchCode, value); }
        public ObservableRangeCollection<PalletHeaderModel> MainPalletHeaderList { get; set; }
        public ObservableRangeCollection<PalletHeaderModel> PalletHeaderList { get; set; }
        public AsyncCommand ApiSearchCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand AddNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletHeaderListVM()
        {
            MainPalletHeaderList = new ObservableRangeCollection<PalletHeaderModel>();
            PalletHeaderList = new ObservableRangeCollection<PalletHeaderModel>();
            ApiSearchCommand = new AsyncCommand(ApiSearch);
            TappedCommand = new AsyncCommand(Tapped);
            AddNavCommand = new AsyncCommand(AddNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task ApiSearch()
        {
            var val = ApiSearchCode.ToUpperInvariant();
            await dependencies.notifService.LoadingProcess("Begin", "Searching...");
            try
            {
                PalletHeaderList.ReplaceRange(await dependencies.serverDbPalletHeaderService.GetList(new PalletHeaderModel { PalletCode = val }, "PalletCode"));
            }
            catch
            {
                await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server.");
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task Tapped()
        {
            if(SelectedItem != null)
            {
                Preferences.Set("PrefSelectedPalletHeader", SelectedItem.PalletCode);
                await Shell.Current.GoToAsync($"{nameof(PalletDetailsListPage)}");
                SelectedItem = null;
            }
        }
        private async Task AddNav()
        {
            Preferences.Remove("PrefPalletAddPageInitialRefresh");
            await Shell.Current.GoToAsync($"{nameof(PalletAddPage)}");
        }

        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            MainPalletHeaderList.Clear();
        }
    }
}
