using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.MainServices;
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
    public class AddDetailPopupVM : ViewModelBase
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        ILocalArticleMasterServices localDbItemMasterService;
        IMainServices mainService;
        IToastNotifService notifyService;
        IncomingDetailModel _e;
        string _scannedCode,_poNumber,_itemCode,_itemDesc,_palletCode,_amount, _errorText;
        int _partialCQTY;
        bool _errorView,_succesView;
        public IncomingDetailModel E { get => _e; set => SetProperty(ref _e, value); }
        public string ScannedCode { get => _scannedCode; set => SetProperty(ref _scannedCode, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string ItemCode { get => _itemCode; set => SetProperty(ref _itemCode, value); }
        public string ItemDesc { get => _itemDesc; set => SetProperty(ref _itemDesc, value); }
        public string Amount { get => _amount; set => SetProperty(ref _amount, value); }
        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }
        public int PartialCQTY { get => _partialCQTY; set => SetProperty(ref _partialCQTY, value); }
        public bool ErrorView { get => _errorView; set => SetProperty(ref _errorView, value); }
        public bool SuccessView { get => _succesView; set => SetProperty(ref _succesView, value); }

        public ObservableRangeCollection<ItemMasterModel> ItemMasterList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }

        public AsyncCommand NavPalletListCommand { get; }
        public AsyncCommand AddDetailCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public AddDetailPopupVM()
        {
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            localDbItemMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            mainService = DependencyService.Get<IMainServices>();
            notifyService = DependencyService.Get<IToastNotifService>();

            ItemMasterList = new ObservableRangeCollection<ItemMasterModel>();
            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();

            NavPalletListCommand = new AsyncCommand(NavPalletList);
            AddDetailCommand = new AsyncCommand(AddDetail);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

            MessagingCenter.Subscribe<PalletListPopupVM, string>(this, "FromPalletListPopupToAdd", async (page, e) =>
            {
                await Task.Delay(1);
                PalletCode = e;
            });
        }
        private async Task NavPalletList()
        {
            await PopupNavigation.Instance.PushAsync(new PalletListPopupPage("AddDetail"));
        }
        private async Task AddDetail()
        {
            var loggedInUser = Preferences.Get("PrefUserId", 0);
            if (!string.IsNullOrWhiteSpace(PalletCode) && PartialCQTY != 0)
            {
                if (loggedInUser == 0)
                {
                    await mainService.RemovePreferences();
                    await notifyService.StaticToastNotif("Error", "User not found.");
                    await Task.Delay(3000);
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                }
                else
                {
                    bool isProceed = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to add the item?", "Yes", "No");
                    if(isProceed == true)
                    {
                        var data = new IncomingPartialDetailModel
                        {
                            INCDetId = E.INCDetId,
                            INCHeaderId = E.INCHeaderId,
                            ItemCode = E.ItemCode,
                            ItemDesc = E.ItemDesc,
                            PartialCQTY = PartialCQTY,
                            PalletCode = PalletCode,
                            UserId = loggedInUser,
                            TimesUpdated = 0,
                            POHeaderNumber = E.POHeaderNumber,
                            Status = "Pending",

                        };
                        await localDbIncomingParDetailService.Insert("RefIdAutoGenerate", data);
                        await notifyService.StaticToastNotif("Success", "Item added.");
                        MessagingCenter.Send(this, "FromDetailsAddMSG", "AddRefresh");
                        await PopupNavigation.Instance.PopAllAsync(true);
                    }
                    
                }
            }
            else
            {
                await notifyService.StaticToastNotif("Error", "Missing entry");
            }
        }
        private async Task AddDetail2()
        {
            if (!string.IsNullOrWhiteSpace(PalletCode) && PartialCQTY != 0)
            {
                E.TimesUpdated++;
                E.PalletCode = PalletCode;
                E.Cqty = PartialCQTY;
                await localDbIncomingDetailService.Update("Common", E);
                await notifyService.StaticToastNotif("Success", "Item added.");
                MessagingCenter.Send(this, "FromDetailsAddMSG", "AddRefresh");
                await PopupNavigation.Instance.PopAllAsync(true);
            }
            else
            {
                await notifyService.StaticToastNotif("Error", "Missing entry");
            }
        }
        private async Task PageRefresh()
        {
            PalletCode = Preferences.Get("PrefSelectedPallet", string.Empty);
            if (!string.IsNullOrWhiteSpace(PONumber) && !string.IsNullOrWhiteSpace(ScannedCode))
            {
                await QueryCode();
            }
        }
        public async Task QueryCode()
        {
            ItemMasterList.Clear();
            IncomingDetailList.Clear();
            string[] a = { ScannedCode };
            var retitems = await localDbItemMasterService.GetList("CaseCode", a, null);
            ItemMasterList.AddRange(retitems);
            if (ItemMasterList.Count > 0)
            {
                foreach (var item in ItemMasterList)
                {
                    string[] b = { PONumber, item.ItemCode };
                    var retIncomingItems = await localDbIncomingDetailService.GetModel("PO,ItemCode", b, null);
                    if (retIncomingItems != null)
                    {
                        IncomingDetailList.Add(retIncomingItems);
                    }
                }
                if (IncomingDetailList.Count == 1)
                {
                    if(IncomingDetailList[0].TimesUpdated == 0)
                    {
                        PONumber = IncomingDetailList[0].POHeaderNumber;
                        ItemCode = IncomingDetailList[0].ItemCode;
                        ItemDesc = IncomingDetailList[0].ItemDesc;
                        Amount = "Pesos :" + IncomingDetailList[0].Amount; ;
                        E = IncomingDetailList[0];
                        await notifyService.StaticToastNotif("Success", "Item found");
                        ErrorView = false;
                        SuccessView = true;
                    }
                    else
                    {
                        //item already registered
                        ErrorText = "Item already added";
                        ErrorView = true;
                        SuccessView = false;
                    }
                }
                else if (IncomingDetailList.Count > 1)
                {
                    // mutiple item detected
                    ErrorText = "Multiple items detected";
                    ErrorView = true;
                    SuccessView = false;
                }
                else
                {
                    // item not found in incoming detail
                    ErrorText = "Item not found in this P.O";
                    ErrorView = true;
                    SuccessView = false;
                }
            }
            else
            {
                // item not found in item master
                ErrorText = "Item not fount in masterlist";
                ErrorView = true;
                SuccessView = false;
            }
        }
    }
}
