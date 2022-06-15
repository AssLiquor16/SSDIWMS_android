using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Views.PalletPages.PalletSubPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.PalletVMs
{
    public class PalletHeaderVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        public GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();
        public ObservableRangeCollection<PalletHeaderModel> PalletHeaderList { get; set; }
        public AsyncCommand AddPalletHeaderPopupNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletHeaderVM()
        {
            PalletHeaderList = new ObservableRangeCollection<PalletHeaderModel>();
            AddPalletHeaderPopupNavCommand = new AsyncCommand(AddPalletHeaderPopupNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task AddPalletHeaderPopupNav()
        {
            Preferences.Remove("PrefPalletAddSubInitialRefresh");
            await Shell.Current.GoToAsync($"{nameof(PalletAddSubPage)}");
        }

        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            await LoadList();
        }
        private async Task LoadList()
        {
            PalletHeaderList.Clear();
            PalletHeaderList.AddRange(await dependencies.localDbPalletHeaderService.GetList());

        }
    }
}
