using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseMaster;
using SSDIWMS_android.Services.NotificationServices;
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
    public class WarehouseLocationMasterListDetailSubModuleVM : ViewModelBase
    {
        IToastNotifService toastNotifService;
        ILocalWarehouseLocationMasterServices localDbWarehouseLocationMasterService;
        ILocalWarehouseMasterServices localDbWarehouseMasterService;

        public LiveTime livetime { get; } = new LiveTime();

        WarehouseLocationModel _selectedWarehouseLocation;
        string _pageCameFrom,_userFullname, _searchString;
        bool _isRefreshing;
        public string SearchString
        {
            get => _searchString;
            set
            {

                if (value == _searchString)
                    return;
                _searchString = value;
                SearchItem(value);
                OnPropertyChanged();
            }
        }
        public string PageCameFrom { get => _pageCameFrom; set => SetProperty(ref _pageCameFrom, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public WarehouseLocationModel SelectedWarehouseLocation { get => _selectedWarehouseLocation; set => SetProperty(ref _selectedWarehouseLocation, value); }

        public ObservableRangeCollection<WarehouseLocationModel> WarehouseLocationList { get; set; }
        public ObservableRangeCollection<WarehouseLocationModel> FilteredWarehouseLocationList { get; set; }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }

        public WarehouseLocationMasterListDetailSubModuleVM()
        {
            toastNotifService = DependencyService.Get<IToastNotifService>();
            localDbWarehouseLocationMasterService = DependencyService.Get<ILocalWarehouseLocationMasterServices>();
            localDbWarehouseMasterService = DependencyService.Get<ILocalWarehouseMasterServices>();
            WarehouseLocationList = new ObservableRangeCollection<WarehouseLocationModel>();
            FilteredWarehouseLocationList = new ObservableRangeCollection<WarehouseLocationModel>();
            TappedCommand = new AsyncCommand(Tapped);
            CancelCommand = new AsyncCommand(Cancel);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

        }

        public async Task Tapped()
        {
            await toastNotifService.LoadingProcess("Begin", "Processing...");
            if (SelectedWarehouseLocation != null)
            {
                if (PageCameFrom == "EditDetail") { MessagingCenter.Send(this, "FromWarehouseLocationListToEdit", SelectedWarehouseLocation.Final_Location); }
                SelectedWarehouseLocation = null;
                await Shell.Current.GoToAsync("..");
            }
            await toastNotifService.LoadingProcess("End");
        }
        public async Task PageRefresh()
        {
            await livetime.LiveTimer();
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
            WarehouseLocationList.Clear();
            var filter = new WarehouseModel
            {
                WarehouseId = Preferences.Get("PrefUserWarehouseAssignedId", 0)
            };
            var wh = await localDbWarehouseMasterService.GetFirstOrDefault(filter);
            var filter2 = new WarehouseLocationModel
            {
                Warehouse = wh.W_LocationInitial
            };
            WarehouseLocationList.AddRange(await localDbWarehouseLocationMasterService.GetList(filter2));
            FilteredWarehouseLocationList.Clear();
            FilteredWarehouseLocationList.AddRange(WarehouseLocationList);
        }
        public async Task ColViewRefresh()
        {
            IsRefreshing = true;
            var filter = new WarehouseModel
            {
                WarehouseId = Preferences.Get("PrefUserWarehouseAssignedId", 0)
            };
            var wh = await localDbWarehouseMasterService.GetFirstOrDefault(filter);
            var filter2 = new WarehouseLocationModel
            {
                Warehouse = wh.W_LocationInitial
            };
            WarehouseLocationList.AddRange(await localDbWarehouseLocationMasterService.GetList(filter2));
            FilteredWarehouseLocationList.Clear();
            FilteredWarehouseLocationList.AddRange(WarehouseLocationList);
            IsRefreshing = false;
        }
        public void SearchItem(string val)
        {
            val = val.ToUpper();
            if (string.IsNullOrWhiteSpace(val))
            {
                FilteredWarehouseLocationList.Clear();
                FilteredWarehouseLocationList.AddRange(WarehouseLocationList);
            }
            else
            {
                FilteredWarehouseLocationList.Clear();
                FilteredWarehouseLocationList.AddRange(WarehouseLocationList.Where(x => x.Final_Location.Contains(val)).ToList());
            }
        }
        private async Task Cancel() => await Shell.Current.GoToAsync("..");
    }
}
