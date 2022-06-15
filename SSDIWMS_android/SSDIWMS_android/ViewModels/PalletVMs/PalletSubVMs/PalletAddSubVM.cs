using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
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
        public ObservableRangeCollection<ItemMasterModel> ToBeAddList { get; set; }
        public AsyncCommand ItemListNavCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletAddSubVM()
        {

            ToBeAddList = new ObservableRangeCollection<ItemMasterModel>();
            ItemListNavCommand = new AsyncCommand(ItemListNav);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task ItemListNav() => await Shell.Current.GoToAsync($"{nameof(PAddItemListPage)}");
        
        private async Task PageRefresh()
        {
            if (Preferences.Get("PrefPalletAddSubInitialRefresh", false) == false)
            {
                await livetime.LiveTimer();
                Preferences.Set("PrefPalletAddSubInitialRefresh", true);
            }
            
        }
        private async Task AddItemToList(ItemMasterModel obj)
        {
            await Task.Delay(10);
            ToBeAddList.Add(obj);
        }
    }
}
