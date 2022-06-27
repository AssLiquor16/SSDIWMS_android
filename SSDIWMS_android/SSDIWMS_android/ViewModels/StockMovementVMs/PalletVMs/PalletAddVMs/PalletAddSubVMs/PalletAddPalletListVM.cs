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
    public class PalletAddPalletListVM : ViewModelBase
    {
        string _searchCode, _apiSearchCode;
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        PalletMasterModel _selectedItem;
        public ObservableRangeCollection<PalletMasterModel> MainPalletMasterList { get; set; }
        public ObservableRangeCollection<PalletMasterModel> PalletMasterList { get; set; }

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
        public PalletMasterModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string ApiSearchCode { get => _apiSearchCode; set => SetProperty(ref _apiSearchCode, value); }

        public AsyncCommand ApiSearchCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletAddPalletListVM()
        {
            MainPalletMasterList = new ObservableRangeCollection<PalletMasterModel>();
            PalletMasterList = new ObservableRangeCollection<PalletMasterModel>();
            ApiSearchCommand = new AsyncCommand(ApiSearch);
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        public async Task ApiSearch()
        {
            if (!string.IsNullOrWhiteSpace(ApiSearchCode))
            {
                var val = ApiSearchCode.ToUpperInvariant();
                await dependencies.notifService.LoadingProcess("Begin", "Searching...");
                try
                {
                    PalletMasterList.Clear();
                    PalletMasterList.AddRange(await dependencies.serverDbTPalletMasterService.GetList(new PalletMasterModel { PalletCode = val , WarehouseId = Preferences.Get("PrefUserWarehouseAssignedId",0) }, "PalletCode/Status=Not-Use/WarehouseId"));
                }
                catch
                {
                    await dependencies.notifService.StaticToastNotif("Error", "Cannot connect to server");
                }
                await dependencies.notifService.LoadingProcess("End");
            }
            else
            {
                PalletMasterList.Clear();
            }
        }
        private async Task Tapped()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Processing...");
            if(SelectedItem != null)
            {
                MessagingCenter.Send(this, "SetPallet", SelectedItem);
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
                PalletMasterList.ReplaceRange(MainPalletMasterList.Where(x => x.PalletCode.Contains(val)).ToList());
            }
            else
            {
                PalletMasterList.ReplaceRange(MainPalletMasterList);
            }
        }
        private async Task PageRefresh()
        {
            await Task.Delay(500);
        }
    }
}