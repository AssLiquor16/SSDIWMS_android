using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.PutAwayVMs.PHTransferToPupSubVMs
{
    public class WhLocListPupVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        public ObservableRangeCollection<WarehouseLocationModel> WarehouseLocList { get; set; }

        string _searchCode;
        WarehouseLocationModel _selectedItem;
        public string SearchCode { get => _searchCode; set => SetProperty(ref _searchCode, value); }
        public WarehouseLocationModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ApiSearchCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public WhLocListPupVM()
        {
            WarehouseLocList = new ObservableRangeCollection<WarehouseLocationModel>();

            TappedCommand = new AsyncCommand(Tapped);
            ApiSearchCommand = new AsyncCommand(ApiSearch);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

        }
        private async Task Tapped()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Processing...");
            if(SelectedItem != null)
            {
                MessagingCenter.Send(this, "SetNewWarehouseLocation", SelectedItem);
                await Task.Delay(100);
                await PopupNavigation.Instance.PopAllAsync(true);
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task ApiSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchCode))
            {
                var val = SearchCode.ToUpperInvariant();
                await dependencies.notifService.LoadingProcess("Begin", "Searching...");
                try
                {
                    var warehouselocs = await dependencies.serverDbTWarehouseLocationService.GetList(new WarehouseLocationModel { Final_Location = val, Warehouse = Preferences.Get("PrefWarehouseInitial", string.Empty) }, "Final_Loc/Warehouse");
                    WarehouseLocList.ReplaceRange(warehouselocs);
                    var currentwhloc = warehouselocs.Where(x => x.Final_Location == Preferences.Get("PrefSelectedPH.CurrentWhLoc", string.Empty)).FirstOrDefault();
                    if (currentwhloc != null)
                    {
                        WarehouseLocList.Remove(currentwhloc);
                    }
                }
                catch
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Cannot connect to server, please try again.", "Ok");
                    await PopupNavigation.Instance.PopAllAsync(true);
                }
                await dependencies.notifService.LoadingProcess("End");
            }
        }
        private async Task PageRefresh()
        {
            await Task.Delay(10);
            WarehouseLocList.Clear();
        }
    }
}
