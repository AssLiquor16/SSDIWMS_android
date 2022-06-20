using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Temp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.PalletVMs.PalletAddVMs.PalletAddSubPopupVMs
{
    public class PalletAddItemDetailPopupVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        ItemWithQtyModel _passedItem;
        public ItemWithQtyModel PasseItem { get => _passedItem; set => SetProperty(ref _passedItem, value); }
        public AsyncCommand SaveItemChangesCommand { get; }
        public AsyncCommand RemoveItemCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PalletAddItemDetailPopupVM()
        {
            RemoveItemCommand = new AsyncCommand(RemoveItem);
            SaveItemChangesCommand = new AsyncCommand(SaveItemChanges);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task RemoveItem()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Removing...");
            if(await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to remove the item?", "Yes", "No") == true)
            {
                MessagingCenter.Send(this, "RemoveItem", PasseItem);
                await Task.Delay(300);
                await PopupNavigation.Instance.PopAsync(true);
            }
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task SaveItemChanges()
        {
            await dependencies.notifService.LoadingProcess("Begin", "Saving...");
            MessagingCenter.Send(this, "EditItem", PasseItem);
            await Task.Delay(300);
            await PopupNavigation.Instance.PopAsync(true);
            await dependencies.notifService.LoadingProcess("End");
        }
        private async Task PageRefresh()
        {
            await Task.Delay(100);
        }
    }
}
