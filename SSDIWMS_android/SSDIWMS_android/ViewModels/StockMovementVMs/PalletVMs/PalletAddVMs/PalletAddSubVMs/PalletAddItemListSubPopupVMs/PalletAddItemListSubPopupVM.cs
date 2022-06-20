using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Temp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubVMs.PalletAddItemListSubPopupVMs
{
    public class PalletAddItemListSubPopupVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        ItemMasterModel _passedItem;
        int _qty;
        DateTime _expiryDate;
        public DateTime _minDate = DateTime.Now;
        public ItemMasterModel PassedItem { get => _passedItem; set => SetProperty(ref _passedItem, value); }
        public int Qty { get => _qty; set => SetProperty(ref _qty, value); }
        public DateTime ExpiryDate { get => _expiryDate; set => SetProperty(ref _expiryDate, value); }
        public DateTime MinDate { get => _minDate; set => SetProperty(ref _minDate, value); }
        public AsyncCommand AddCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletAddItemListSubPopupVM()
        {
            AddCommand = new AsyncCommand(Add);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Add()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Processing...");
            if (Qty > 0)
            {
                MessagingCenter.Send(this, "AddItem", new ItemWithQtyModel { Item = PassedItem, Qty = Qty, DateAdded = DateTime.Now, ExpiryDate = ExpiryDate });
                await Task.Delay(300);
                if (await App.Current.MainPage.DisplayAlert("Alert", "Item has been added to the list, do you want to add more item?", "Yes", "No") == false)
                {
                    await Shell.Current.GoToAsync($"..");
                    await PopupNavigation.Instance.PopAsync(true);
                }
                else
                {
                    await PopupNavigation.Instance.PopAsync(true);
                }
                MessagingCenter.Send(this, "SelectedItemNull", "Go");
                await Task.Delay(300);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Qty must be greater than 0", "Ok");
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task PageRefresh()
        {
            await Task.Delay(1000);
            ExpiryDate = DateTime.Now;
        }
    }
}
