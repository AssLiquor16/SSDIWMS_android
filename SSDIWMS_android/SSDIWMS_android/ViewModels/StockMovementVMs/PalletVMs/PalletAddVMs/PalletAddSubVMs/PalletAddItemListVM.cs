using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Temp;
using SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs.PalletAddItemListSubPopupVMs;
using SSDIWMS_android.Views.StockMovementPages.PalletPages.PalletAddPages.PalletAddSubPages.PalletAddItemListSubPopupPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs
{
    public class PalletAddItemListVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        ItemMasterModel _selectedItem;
        string _searchCode;
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
        public ItemMasterModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public ObservableRangeCollection<ItemMasterModel> MainItemList { get; set; }
        public ObservableRangeCollection<ItemMasterModel> ItemList { get; set; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletAddItemListVM()
        {
            
            MainItemList = new ObservableRangeCollection<ItemMasterModel>();
            ItemList = new ObservableRangeCollection<ItemMasterModel>();
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            MessagingCenter.Subscribe<PalletAddItemListSubPopupVM, string>(this, "SelectedItemNull", (sender, e) =>
             {
                 SelectedItem = null;
             });

        }
        private async Task Tapped()
        {
            if(SelectedItem != null)
            {
                await PopupNavigation.Instance.PushAsync(new PalletAddItemListSubPopupPage(SelectedItem));
            }
        }

        private void Search(string val)
        {
            val = val.ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(val))
            {
                ItemList.ReplaceRange(MainItemList.Where(x => x.ItemCode == val || x.CaseCode == val).ToList());
            }
            else
            {
                ItemList.ReplaceRange(MainItemList);
            }
        }
        private async Task PageRefresh()
        {
            MainItemList.Clear();
            MainItemList.AddRange(await dependencies.localDbArticleMasterService.GetList("All", null, null));
            ItemList.ReplaceRange(MainItemList);
        }
    }
}
/*{
    await dependencies.notifService.LoadingProcess("Begin", "Processing...");
    if (SelectedItem != null)
    {
        var sqty = await App.Current.MainPage.DisplayPromptAsync($"{SelectedItem.ItemCode}", $"{SelectedItem.ItemDesc}", "Add", keyboard: Xamarin.Forms.Keyboard.Numeric);
        if (!string.IsNullOrWhiteSpace(sqty))
        {
            var qty = Convert.ToInt32(sqty);
            if (qty > 0)
            {
                MessagingCenter.Send(this, "AddItem", new ItemWithQtyModel { Item = SelectedItem, Qty = qty, DateAdded = DateTime.Now });
                await Task.Delay(300);
                if (await App.Current.MainPage.DisplayAlert("Alert", "Item has been added to the list, do you want to add more item?", "Yes", "No") == false)
                {
                    await Shell.Current.GoToAsync($"..");
                }
            }
            else
            {
                await dependencies.notifService.StaticToastNotif("Error", "Item cannot quantity must be more than 0.");
            }
        }
    }
    SelectedItem = null;
    await dependencies.notifService.LoadingProcess("End");
}*/