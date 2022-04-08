using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.TransactionModels;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs
{
    public class HeaderVM : ViewModelBase
    {
        ILocalPalletMasterServices localDbPalletMasterService;

        string _searchText;
        PalletMasterModel _selectedItem;
        bool _isrefreshing;

        public string SearchText
        {
            get => _searchText;
            set
            {

                if (value == _searchText)
                    return;
                _searchText = value;
                Search();
                OnPropertyChanged();
                
            }
        }
        public PalletMasterModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public bool IsRefreshing { get => _isrefreshing; set => SetProperty(ref _isrefreshing, value); }

        public ObservableRangeCollection<PalletMasterModel> PalletMasterList { get; set; }
        public ObservableRangeCollection<PalletMasterModel> FilterPalletMasterList { get; set; }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public AsyncCommand CVRefreshCommand { get; }
        public HeaderVM()
        {
            localDbPalletMasterService = DependencyService.Get<ILocalPalletMasterServices>();

            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            CVRefreshCommand = new AsyncCommand(CVRefresh);
            PalletMasterList = new ObservableRangeCollection<PalletMasterModel>();
            FilterPalletMasterList = new ObservableRangeCollection<PalletMasterModel>();
        }
        private async Task Tapped()
        {
            if(SelectedItem == null)
            {
                return;
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "You select" + SelectedItem.PalletCode, "Ok");
                SelectedItem = null;
            }
        }
        private async Task PageRefresh()
        {
                await Task.Delay(1000);
                await LiveTimer();
                PalletMasterList.Clear();
            try
            {
                var items = await localDbPalletMasterService.GetList("All", null, null);
                PalletMasterList.AddRange(items);
            }
            catch
            {
                PalletMasterList.Clear();
            }
                
                SearchText = string.Empty;
            
            
        }
        private async Task CVRefresh()
        {
            IsRefreshing = true;
            PalletMasterList.Clear();
            try
            {
                var items = await localDbPalletMasterService.GetList("All", null, null);
                PalletMasterList.AddRange(items);
            }
            catch
            {
                PalletMasterList.Clear();
            }
            SearchText = string.Empty;
            IsRefreshing = false;
        }


        private void Search()
        {
            
            Task.Delay(100);
            SearchText = SearchText.ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilterPalletMasterList.Clear();
                FilterPalletMasterList.AddRange(PalletMasterList);
            }
            else
            {
                
                var filter = PalletMasterList.Where(x => x.PalletCode.Contains(SearchText) || x.PalletDescription.Contains(SearchText)).ToList();
                FilterPalletMasterList.Clear();
                FilterPalletMasterList.AddRange(filter);
            }

        }


        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt");
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
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
