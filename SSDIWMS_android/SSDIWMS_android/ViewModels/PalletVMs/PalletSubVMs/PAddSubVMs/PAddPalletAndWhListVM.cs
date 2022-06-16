using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SSDIWMS_android.ViewModels.PalletVMs.PalletSubVMs.PAddSubVMs
{
    public class PAddPalletAndWhListVM : ViewModelBase
    {
        GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();

        public ObservableRangeCollection<PalletMasterModel> MainPalletList { get; set; }
        public ObservableRangeCollection<PalletMasterModel> PalletList { get; set; }
        public ObservableRangeCollection<WarehouseLocationModel> MainWarehouseLocList { get; set; }
        public ObservableRangeCollection<WarehouseLocationModel> WarehouseLocList { get; set; }

        bool _setInfoBtnVisible;
        PalletMasterModel _selectedPallet;
        WarehouseLocationModel _selectedWarehouseLoc;
        public PalletMasterModel SelectedPallet { get => _selectedPallet; set => SetProperty(ref _selectedPallet, value); }
        public WarehouseLocationModel SelectedWarehouseLoc { get => _selectedWarehouseLoc; set => SetProperty(ref _selectedWarehouseLoc, value); }
        public bool SetInfoBtnVisible { get => _setInfoBtnVisible; set => SetProperty(ref _setInfoBtnVisible, value); }
        public AsyncCommand SetInfoCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PAddPalletAndWhListVM()
        {
            SetInfoCommand = new AsyncCommand(SetInfo);
            TappedCommand = new AsyncCommand(Tapped);
            MainPalletList = new ObservableRangeCollection<PalletMasterModel>();
            PalletList = new ObservableRangeCollection<PalletMasterModel>();
            MainWarehouseLocList = new ObservableRangeCollection<WarehouseLocationModel>();
            WarehouseLocList = new ObservableRangeCollection<WarehouseLocationModel>();

            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task SetInfo()
        {
            if (SelectedPallet != null && SelectedWarehouseLoc != null)
            {
                SetInfoBtnVisible = true;
            }
            else
            {
                SetInfoBtnVisible = false;
            }
        }
        private async Task Tapped()
        {
            if (SelectedPallet != null && SelectedWarehouseLoc != null)
            {
                SetInfoBtnVisible = true;
            }
            else
            {
                SetInfoBtnVisible = false;
            }
        }
        private async Task PageRefresh()
        {
            SetInfoBtnVisible = false;
            MainPalletList.Clear();
            MainWarehouseLocList.Clear();
            MainPalletList.AddRange(await dependencies.localDbPalletMasterService.GetList("WhIdFilter", null, new int[] { Preferences.Get("PrefUserWarehouseAssignedId", 0) }));
            var wh = await dependencies.localDbWarehouseService.GetFirstOrDefault(new WarehouseModel { WarehouseId = Preferences.Get("PrefUserWarehouseAssignedId", 0) });
            MainWarehouseLocList.AddRange(await dependencies.localDbWarehouseLocationService.GetList(new WarehouseLocationModel { Warehouse = wh.W_LocationInitial }, "Warehouse"));
            PalletList.ReplaceRange(MainPalletList);
            WarehouseLocList.ReplaceRange(MainWarehouseLocList);
        }
    }
}
