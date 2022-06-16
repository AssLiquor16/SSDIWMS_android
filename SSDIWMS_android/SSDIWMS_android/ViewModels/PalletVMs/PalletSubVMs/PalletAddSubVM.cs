using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Temp;
using SSDIWMS_android.ViewModels.PalletVMs.PalletSubVMs.PAddSubVMs;
using SSDIWMS_android.Views.PalletPages.PalletSubPages.PAddSubPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.PalletVMs.PalletSubVMs
{
    public class PalletAddSubVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        public GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();
        public ObservableRangeCollection<ItemWithQtyModel> ToBeAddList { get; set; }
        string _palletCode, _warehouseLocation;
        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }
        public string WarehouseLocation { get => _warehouseLocation; set => SetProperty(ref _warehouseLocation, value); }
        public AsyncCommand EditheadersNavCommand { get; }
        public AsyncCommand ItemListNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }

        public PalletAddSubVM()
        {
            ToBeAddList = new ObservableRangeCollection<ItemWithQtyModel>();
            EditheadersNavCommand = new AsyncCommand(EditheadersNav);
            ItemListNavCommand = new AsyncCommand(ItemListNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            MessagingCenter.Subscribe<PAddItemListVM, ItemWithQtyModel>(this, "AddToList", async (page, e) =>
              {
                  await AddItemToList(e);
              });
        }
        private async Task ItemListNav()
        {
            Preferences.Remove("PrefPAddItemListInitialRefresh");
            await Shell.Current.GoToAsync($"{nameof(PAddItemListPage)}");
        }
        private async Task EditheadersNav()
        {
            Preferences.Remove("PAddPalletWhListVMInitialRefresh");
            await Shell.Current.GoToAsync($"{nameof(PAddPalletAndWhListPage)}");
        }
        private async Task PageRefresh()
        {
            if (Preferences.Get("PrefPalletAddSubInitialRefresh", false) == false)
            {
                await livetime.LiveTimer();
                Preferences.Set("PrefPalletAddSubInitialRefresh", true);
            }
            
        }
        private async Task AddItemToList(ItemWithQtyModel obj)
        {
            await Task.Delay(10);
            ToBeAddList.Add(obj);
        }
    }
}
