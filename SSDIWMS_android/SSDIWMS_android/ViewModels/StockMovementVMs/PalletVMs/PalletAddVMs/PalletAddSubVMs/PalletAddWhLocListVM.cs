using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs
{
    public class PalletAddWhLocListVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        WarehouseLocationModel _selectedItem;
        string _searchCode,_apisearchCode;
        public string SearchCode
        {
            get => _searchCode;
            set
            {
                if (value == _searchCode)
                    return;
                _searchCode = value;
                OnPropertyChanged();
                Search(value);
            }
        }
        public string ApiSearchCode { get => _apisearchCode; set => SetProperty(ref _apisearchCode, value); }
        public ObservableRangeCollection<WarehouseLocationModel> MainWarehouseLocList { get; set; }
        public ObservableRangeCollection<WarehouseLocationModel> WarehouseLocList { get; set; }
        public WarehouseLocationModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public AsyncCommand ApiSearchCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand {get;}
        public PalletAddWhLocListVM()
        {
            MainWarehouseLocList = new ObservableRangeCollection<WarehouseLocationModel>();
            WarehouseLocList = new ObservableRangeCollection<WarehouseLocationModel>();
            ApiSearchCommand = new AsyncCommand(ApiSearch);
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task ApiSearch()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Searching...");
            try
            {
                var val = ApiSearchCode.ToUpperInvariant();
                var data = new WarehouseLocationModel
                {
                    Warehouse = Preferences.Get("PrefWarehouseInitial", string.Empty),
                    Final_Location = val
                };
                WarehouseLocList.ReplaceRange(await dependencies.serverDbTWarehouseLocationService.GetList(data, "Final_Loc/Warehouse"));
            }
            catch
            {
                await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server");
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task Tapped()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Processing...");
            if(SelectedItem != null)
            {
                MessagingCenter.Send(this, "SetWarehouseLoc", SelectedItem);
                await Task.Delay(300);
                await Shell.Current.GoToAsync($"..");
                SelectedItem = null;
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private void Search(string val)
        {
            val = val.ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(val))
            {
                WarehouseLocList.ReplaceRange(MainWarehouseLocList.Where(x => x.Final_Location.Contains(val)).ToList());
            }
            else
            {
                WarehouseLocList.ReplaceRange(MainWarehouseLocList);
            }
        }
        private async Task PageRefresh()
        {
            MainWarehouseLocList.Clear();
            
        }
    }
}
