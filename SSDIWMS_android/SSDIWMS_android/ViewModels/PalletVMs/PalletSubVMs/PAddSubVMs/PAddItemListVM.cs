using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.PalletVMs.PalletSubVMs.PAddSubVMs
{
    public class PAddItemListVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        public GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();

        string _searchCode;
        ItemMasterModel _selectedItem;
        public ItemMasterModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
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
        public ObservableRangeCollection<ItemMasterModel> MainItemMasterList { get; }
        public ObservableRangeCollection<ItemMasterModel> ItemMasterList { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PAddItemListVM()
        {
            MainItemMasterList = new ObservableRangeCollection<ItemMasterModel>();
            ItemMasterList = new ObservableRangeCollection<ItemMasterModel>();
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private void Search(string val)
        {
            val = val.ToUpperInvariant();
            var a = MainItemMasterList.Where(x => x.ItemCode == val || x.CaseCode == val).ToList();
            if(a.Count() > 0)
            {
                ItemMasterList.ReplaceRange(a);
            }
            else
            {
                ItemMasterList.ReplaceRange(MainItemMasterList);
            }
        }
        private async Task Tapped()
        {
            if(SelectedItem != null)
            {
                var sqty = await App.Current.MainPage.DisplayPromptAsync($"{SelectedItem.ItemCode}", $"{SelectedItem.ItemDesc}", keyboard: Keyboard.Numeric, placeholder: "Enter quantity");
                if (!string.IsNullOrEmpty(sqty))
                {
                    var qty = Convert.ToInt32(sqty);
                    if (qty != 0)
                    {
                        MessagingCenter.Send(this, "AddToList", new ItemWithQtyModel { Item = SelectedItem, Qty = qty });
                        if (await App.Current.MainPage.DisplayAlert("Alert", "Item added, do you want to add another item?", "Yes", "No") == false)
                        {
                            await Shell.Current.GoToAsync("..");
                        }
                        else
                        {
                            SearchCode = string.Empty;
                        }
                    }
                    else
                    {
                        await dependencies.notifService.StaticToastNotif("Error", "No quantity");
                    }
                }
                SelectedItem = null;
            }
        }
        private async Task PageRefresh()
        {
            if(Preferences.Get("PrefPAddItemListInitialRefresh", false) == false)
            {
                await livetime.LiveTimer();
                ItemMasterList.Clear();
                MainItemMasterList.Clear();
                MainItemMasterList.AddRange(await dependencies.localDbArticleMasterService.GetList("All", null, null));
                ItemMasterList.AddRange(MainItemMasterList);
                Preferences.Set("PrefPAddItemListInitialRefresh", true);
            }
        }
    }
}
