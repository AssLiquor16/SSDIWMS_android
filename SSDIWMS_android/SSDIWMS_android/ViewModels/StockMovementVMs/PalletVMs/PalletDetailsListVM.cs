using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs
{
    public class PalletDetailsListVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        public LiveTime livetime { get; } = new LiveTime();
        public ObservableRangeCollection<PalletDetailsModel> MainPalletDetailsList { get; set; }
        public ObservableRangeCollection<PalletDetailsModel> PalletDetailsList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletDetailsListVM()
        {
            MainPalletDetailsList = new ObservableRangeCollection<PalletDetailsModel>();
            PalletDetailsList = new ObservableRangeCollection<PalletDetailsModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            MainPalletDetailsList.Clear();
            MainPalletDetailsList.AddRange(await dependencies.localDbPalletDetailsService.GetList(new PalletDetailsModel { PalletCode = Preferences.Get("PrefSelectedPalletHeader", string.Empty) }, "PalletCode"));
            PalletDetailsList.ReplaceRange(MainPalletDetailsList);
        }
    }   
}
