using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdatePalletMaster;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages.IncomingDetailSubPopupModulePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs.IncomingDetailSubPopupModuleVMs
{
    public class PalletListPopupVM : ViewModelBase
    {
        ILocalPalletMasterServices localDbPalletMasterService;

        PalletMasterModel _selectedPallet;
        string _pageCameFrom, _searchString;
        bool _isRefreshing;
        public PalletMasterModel SelectedPallet { get => _selectedPallet; set => SetProperty(ref _selectedPallet, value); }
        public string PageCameFrom { get => _pageCameFrom; set => SetProperty(ref _pageCameFrom, value); }
        public string SearchString
        {
            get => _searchString;
            set
            {

                if (value == _searchString)
                    return;
                _searchString = value;
                OnPropertyChanged();
                SearchItem();
            }
        }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public ObservableRangeCollection<PalletMasterModel> PalletList { get; set; }
        public ObservableRangeCollection<PalletMasterModel> FilteredPalletList { get; set; }
        public AsyncCommand UpdatePalletNavCommand { get; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand TappedCommand { get; set; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletListPopupVM()
        {
            localDbPalletMasterService = DependencyService.Get<ILocalPalletMasterServices>();
            PalletList = new ObservableRangeCollection<PalletMasterModel>();
            FilteredPalletList = new ObservableRangeCollection<PalletMasterModel>();
            UpdatePalletNavCommand = new AsyncCommand(UpdatePalletNav);
            CancelCommand = new AsyncCommand(Close);
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task UpdatePalletNav()
        {
            await PopupNavigation.Instance.PushAsync(new PalletMasterUpdaterPopupPage());
            SearchString = string.Empty;
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async Task Tapped()
        {
            if (SelectedPallet != null)
            {
                if (PageCameFrom == "AddDetail")
                {
                    Preferences.Set("PrefSelectedPallet", SelectedPallet.PalletCode);
                    MessagingCenter.Send(this, "FromPalletListPopupToAdd", SelectedPallet.PalletCode);
                    await PopupNavigation.Instance.PopAsync(true);
                }
                else if( PageCameFrom == "EditDetail")
                {
                    Preferences.Set("PrefSelectedPallet", SelectedPallet.PalletCode);
                    MessagingCenter.Send(this, "FromPalletListPopupToEdit", SelectedPallet.PalletCode);
                    await PopupNavigation.Instance.PopAsync(true);
                }
                else
                {
                    await PopupNavigation.Instance.PopAllAsync(true);
                }
            }
        }
        private async Task ColViewRefresh()
        {
            IsRefreshing = true;
            SearchString = string.Empty;
            PalletList.Clear();
            FilteredPalletList.Clear();
            var e = await localDbPalletMasterService.GetList("WhIdFilter", null, null);
            PalletList.AddRange(e);
            FilteredPalletList.AddRange(PalletList);
            IsRefreshing = false;
        }
        private async Task PageRefresh()
        {
            FilteredPalletList.Clear();
            PalletList.Clear();
            var e = await localDbPalletMasterService.GetList("WhIdFilter", null, null);
            PalletList.AddRange(e);
            FilteredPalletList.AddRange(PalletList);
        }
        public async Task Close()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private void SearchItem()
        {
            SearchString = SearchString.ToUpperInvariant();
            FilteredPalletList.Clear();
            if (string.IsNullOrWhiteSpace(SearchString))
            {
                FilteredPalletList.AddRange(PalletList);
            }
            else
            {
                var e = PalletList.Where(x => x.PalletCode.Contains(SearchString) || x.PalletDescription.Contains(SearchString)).ToList();
                FilteredPalletList.AddRange(e);
            }
        }
        public async Task Back()
        {
            await PopupNavigation.Instance.PushAsync(new PalletListPopupPage("PalletList"));
        }
        
    }
}
