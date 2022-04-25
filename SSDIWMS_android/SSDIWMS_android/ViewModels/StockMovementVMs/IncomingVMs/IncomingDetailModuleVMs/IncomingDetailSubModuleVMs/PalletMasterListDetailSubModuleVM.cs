using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Updater.MasterDatas.UpdatePalletMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailModuleVMs.IncomingDetailSubModuleVMs
{
    [QueryProperty(nameof(PageCameFrom), nameof(PageCameFrom))]
    public class PalletMasterListDetailSubModuleVM : ViewModelBase
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
        public PalletMasterListDetailSubModuleVM()
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
            await Shell.Current.GoToAsync("..");
            SearchString = string.Empty;
        }
        private async Task Tapped()
        {
            if (SelectedPallet != null)
            {
                if (PageCameFrom == "AddDetail")
                {
                    Preferences.Set("PrefSelectedPallet", SelectedPallet.PalletCode);
                    MessagingCenter.Send(this, "FromPalletListToAdd", SelectedPallet.PalletCode);
                    await Shell.Current.GoToAsync("..");
                }
                else if (PageCameFrom == "EditDetail")
                {
                    MessagingCenter.Send(this, "FromPalletListToEdit", SelectedPallet.PalletCode);
                    await Shell.Current.GoToAsync("..?DataSender=");
                }
                else
                {
                    await Shell.Current.GoToAsync("..");
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
            await LiveTimer();
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
            FilteredPalletList.Clear();
            PalletList.Clear();
            var e = await localDbPalletMasterService.GetList("WhIdFilter", null, null);
            PalletList.AddRange(e);
            FilteredPalletList.AddRange(PalletList);
        }
        public async Task Close()
        {
            await Shell.Current.GoToAsync("..");
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
            await Shell.Current.GoToAsync("..");
        }

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt"), _userFullname;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
        private async Task LiveTimer()
        {
            await Task.Delay(1);
            Device.StartTimer(TimeSpan.FromSeconds(_datetimeTick), () => {
                Task.Run(async () =>
                {
                    await Task.Delay(1);
                    LiveDate = DateTime.Now.ToString(_datetimeFormat);
                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }
    }
}
