using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletPopupVMs
{
    public class PalletDetailsListPopupVM : ViewModelBase
    {
        string _palletHeader;
        bool _isRefreshing;
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public string PalletHeader { get => _palletHeader; set => SetProperty(ref _palletHeader, value); }
        public ObservableRangeCollection<PalletDetailsModel> MainPalletDetailsList { get; set; }
        public ObservableRangeCollection<PalletDetailsModel> PalletDetailsList { get; set; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletDetailsListPopupVM()
        {
            MainPalletDetailsList = new ObservableRangeCollection<PalletDetailsModel>();
            PalletDetailsList = new ObservableRangeCollection<PalletDetailsModel>();
            ColViewRefreshCommand = new AsyncCommand(ColviewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Loading...");
            try
            {
                MainPalletDetailsList.Clear();
                PalletDetailsList.Clear();
                PalletHeader = Preferences.Get("PrefSelectedPalletHeader", string.Empty);
                var palletdetails = await dependencies.serverDbPalletDetailsService.GetList(new PalletDetailsModel { PalletCode = PalletHeader.ToUpperInvariant() }, "PalletCode");
                MainPalletDetailsList.AddRange(palletdetails);
                PalletDetailsList.ReplaceRange(MainPalletDetailsList);
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Cannot connect to server.", "Ok");
                await PopupNavigation.Instance.PopAsync(true);
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task ColviewRefresh()
        {
            await PageRefresh();
            IsRefreshing = false;
        }
    }
}
