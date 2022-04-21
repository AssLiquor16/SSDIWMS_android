using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs.IncomingDetailSubPopupModuleVMs;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages.IncomingDetailSubPopupModulePages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs
{
    public class EditDetailPopupVM : ViewModelBase
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        IToastNotifService notifyService;
        IncomingPartialDetailModel _e;
        DateTime _expiryDate;
        string _poNumber, _itemCode, _itemDesc, _palletCode, _amount;
        int _partialCQTY;
        public IncomingPartialDetailModel E { get => _e; set => SetProperty(ref _e, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string ItemCode { get => _itemCode; set => SetProperty(ref _itemCode, value); }
        public string ItemDesc { get => _itemDesc; set => SetProperty(ref _itemDesc, value); }
        public string Amount { get => _amount; set => SetProperty(ref _amount, value); }
        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }
        public DateTime ExpiryDate { get => _expiryDate; set => SetProperty(ref _expiryDate, value); }
        public int PartialCQTY { get => _partialCQTY; set => SetProperty(ref _partialCQTY, value); }

        public AsyncCommand NavPalletListCommand { get; }
        public AsyncCommand EditDetailCommand { get; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public EditDetailPopupVM()
        {
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            notifyService = DependencyService.Get<IToastNotifService>();
            CancelCommand = new AsyncCommand(Cancel);
            NavPalletListCommand = new AsyncCommand(NavPalletList);
            EditDetailCommand = new AsyncCommand(EditDetail);

            MessagingCenter.Subscribe<PalletListPopupVM, string>(this, "FromPalletListPopupToEdit", async (page, e) =>
            {
                await Task.Delay(1);
                PalletCode = e;
            });
        }
        private async Task Cancel()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async Task NavPalletList()
        {
            await PopupNavigation.Instance.PushAsync(new PalletListPopupPage("EditDetail"));
        }
        private async Task EditDetail()
        {
            if (E != null)
            {
                if (!string.IsNullOrWhiteSpace(PalletCode) && PartialCQTY != 0)
                {
                    E.TimesUpdated++;
                    E.PalletCode = PalletCode;
                    E.PartialCQTY = PartialCQTY;
                    E.ExpiryDate = ExpiryDate;
                    await localDbIncomingParDetailService.Update("Common", E);
                    await notifyService.StaticToastNotif("Success", "Detail updated succesfully");
                    MessagingCenter.Send(this, "FromDetailsEditMSG", "EditRefresh");
                    await PopupNavigation.Instance.PopAllAsync(true);
                }

            }
        }
        
    }
}
