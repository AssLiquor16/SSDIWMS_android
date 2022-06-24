using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Views.StockMovementPages.StockTransferPages.STPalletToLocationPages.PutAwayPages.PHTransferToPupSubPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.PutAwayVMs
{
    public class PHTransferToVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        GlobalDependencyServices dependencies = new GlobalDependencyServices();

        WarehouseLocationModel _selectedWhLoc;
        public WarehouseLocationModel SelectedWhLoc { get => _selectedWhLoc; set => SetProperty(ref _selectedWhLoc, value); }
        public AsyncCommand WarehouseLocationNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PHTransferToVM()
        {
            WarehouseLocationNavCommand = new AsyncCommand(WarehouseLocationNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task WarehouseLocationNav()
        {
            await PopupNavigation.Instance.PushAsync(new WhLocListPupPage());
        }
        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
        }
    }
}
