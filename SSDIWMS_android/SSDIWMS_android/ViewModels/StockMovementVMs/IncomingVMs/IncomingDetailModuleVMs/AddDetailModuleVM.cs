using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailModuleVMs
{
    [QueryProperty(nameof(PONumber), nameof(PONumber))]
    [QueryProperty(nameof(ScannedCode), nameof(ScannedCode))]
    public class AddDetailModuleVM : ViewModelBase
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        ILocalArticleMasterServices localDbItemMasterService;
        IMainServices mainService;
        IToastNotifService notifyService;
        IncomingDetailModel _e;
        DateTime _expiryDate = DateTime.Now.Date;
        string _scannedCode, _poNumber, _itemCode, _itemDesc, _palletCode, _amount, _errorText;
        int _partialCQTY;
        bool _errorView, _succesView;
        public IncomingDetailModel E { get => _e; set => SetProperty(ref _e, value); }
        public string ScannedCode { get => _scannedCode; set => SetProperty(ref _scannedCode, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string ItemCode { get => _itemCode; set => SetProperty(ref _itemCode, value); }
        public string ItemDesc { get => _itemDesc; set => SetProperty(ref _itemDesc, value); }
        public string Amount { get => _amount; set => SetProperty(ref _amount, value); }
        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }
        public DateTime ExpiryDate { get => _expiryDate; set => SetProperty(ref _expiryDate, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }
        public int PartialCQTY { get => _partialCQTY; set => SetProperty(ref _partialCQTY, value); }
        public bool ErrorView { get => _errorView; set => SetProperty(ref _errorView, value); }
        public bool SuccessView { get => _succesView; set => SetProperty(ref _succesView, value); }

        public ObservableRangeCollection<ItemMasterModel> ItemMasterList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }

        public AsyncCommand NavPalletListCommand { get; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand AddDetailCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public AddDetailModuleVM()
        {
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            localDbItemMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            mainService = DependencyService.Get<IMainServices>();
            notifyService = DependencyService.Get<IToastNotifService>();

            ItemMasterList = new ObservableRangeCollection<ItemMasterModel>();
            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();

            AddDetailCommand = new AsyncCommand(AddDetail);
            CancelCommand = new AsyncCommand(Cancel);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

        }
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
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
                    if (isProceed == true)
                    {
                        ExpiryDate = ExpiryDate.Date;
                        var data = new IncomingPartialDetailModel
                        {
                            INCDetId = E.INCDetId,
                            INCHeaderId = E.INCHeaderId,
                            ItemCode = E.ItemCode,
                            ItemDesc = E.ItemDesc,
                            PartialCQTY = PartialCQTY,
                            PalletCode = PalletCode,
                            ExpiryDate = ExpiryDate,
                            TimesUpdated = 0,
                            POHeaderNumber = E.POHeaderNumber,
                            Status = "Ongoing",
                        };
                        await localDbIncomingParDetailService.Insert("RefIdAutoGenerate", data);
                        await notifyService.StaticToastNotif("Success", "Item added.");
                        MessagingCenter.Send(this, "FromDetailsAddMSG", "AddRefresh");
                        await Shell.Current.GoToAsync("..");
                    }

                }
            }
            else
            {
                await notifyService.StaticToastNotif("Error", "Missing entry");
            }
        }
        private async Task PageRefresh()
        {
            await LiveTimer();
            UserFullName = Preferences.Get("PrefUserFullname", string.Empty);
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
                    PONumber = IncomingDetailList[0].POHeaderNumber;
                    ItemCode = IncomingDetailList[0].ItemCode;
                    ItemDesc = IncomingDetailList[0].ItemDesc;
                    Amount = "Pesos :" + IncomingDetailList[0].Amount; ;
                    E = IncomingDetailList[0];
                    await notifyService.StaticToastNotif("Success", "Item found");
                    ErrorView = false;
                    SuccessView = true;
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
                ErrorText = "Item not found in masterlist";
                ErrorView = true;
                SuccessView = false;
            }
        }



        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt"), _userFullname;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
        private async Task LiveTimer()
        {
            await Task.Delay(1);
            Device.StartTimer(TimeSpan.FromSeconds(_datetimeTick), () => {
                Task.Run(async () =>
                {
                    await Task.Delay(1);
                    LiveDate = DateTime.Now.ToString(_datetimeFormat);
                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }
    }
}
