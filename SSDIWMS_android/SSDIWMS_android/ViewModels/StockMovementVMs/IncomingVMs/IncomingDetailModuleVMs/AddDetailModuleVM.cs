using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailModuleVMs.IncomingDetailSubModuleVMs;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailModulePages.IncomingDetailSubModulePages;
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
        ItemMasterModel _selectedItem;
        DateTime _expiryDate = DateTime.Now.Date;
        string _scannedCode, _poNumber, _itemCode, _itemDesc, _palletCode, _amount, _errorText;
        int _partialCQTY;
        bool _errorView, _succesView, _multipleRowVisible;
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
        public bool MultipleRowVisible { get => _multipleRowVisible; set => SetProperty(ref _multipleRowVisible, value); }
        public bool ErrorView { get => _errorView; set => SetProperty(ref _errorView, value); }
        public bool SuccessView { get => _succesView; set => SetProperty(ref _succesView, value); }

        public ItemMasterModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }


        public ObservableRangeCollection<ItemMasterModel> ItemMasterList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }
        public ObservableRangeCollection<ItemMasterModel> MultipleItemMasterList { get; set; }

        public AsyncCommand TappedCommand { get; }
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
            MultipleItemMasterList = new ObservableRangeCollection<ItemMasterModel>();

            TappedCommand = new AsyncCommand(Tapped);
            NavPalletListCommand = new AsyncCommand(NavPalletList);
            AddDetailCommand = new AsyncCommand(AddDetail);
            CancelCommand = new AsyncCommand(Cancel);
            PageRefreshCommand = new AsyncCommand(PageRefresh);


            MessagingCenter.Subscribe<PalletMasterListDetailSubModuleVM, string>(this, "FromPalletListToAdd", (page, e) =>
            {
                PalletCode = e;
            });

        }
        private async Task Tapped()
        {
            var selected = SelectedItem;
            if(selected != null)
            {
                string[] b = { PONumber, SelectedItem.ItemCode };
                var retIncomingItem = await localDbIncomingDetailService.GetModel("PO,ItemCode", b, null);
                if(retIncomingItem != null)
                {
                    PONumber = retIncomingItem.POHeaderNumber;
                    ItemCode = retIncomingItem.ItemCode;
                    ItemDesc = retIncomingItem.ItemDesc;
                    Amount = "Pesos :" + retIncomingItem.Amount; ;
                    E = retIncomingItem;
                    ErrorView = false;
                    SuccessView = true;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Selected item is null.", "Ok");
                await Cancel();
            }
        }
        private async Task NavPalletList()
        {

            var route = $"{nameof(PalletMasterListDetailSubModulePage)}?PageCameFrom=AddDetail";
            await Shell.Current.GoToAsync(route);
        }
        private async Task Cancel() => await Shell.Current.GoToAsync("..");
        private async Task AddDetail()
        {
            var loggedInUser = Preferences.Get("PrefUserId", 0);
            if (PartialCQTY != 0) // string.IsNullOrWhiteSpace(PalletCode)
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
                            PalletCode = string.Empty, // PalletCode
                            ExpiryDate = DateTime.MinValue.Date, // ExpiryDate
                            TimesUpdated = 0,
                            POHeaderNumber = E.POHeaderNumber,
                            Status = "Ongoing",
                            WarehouseLocation = string.Empty,
                            DateSync = DateTime.Now,
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
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
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
                    // success
                    PONumber = IncomingDetailList[0].POHeaderNumber;
                    ItemCode = IncomingDetailList[0].ItemCode;
                    ItemDesc = IncomingDetailList[0].ItemDesc;
                    Amount = "Pesos :" + IncomingDetailList[0].Amount; ;
                    E = IncomingDetailList[0];
                    ErrorView = false;
                    SuccessView = true;
                }
                else if (IncomingDetailList.Count > 1)
                {
                    // mutiple item detected
                    ErrorText = "Multiple items detected";
                    MultipleRowVisible = false;
                    MultipleItemMasterList.AddRange(ItemMasterList);
                    ErrorView = true;
                    SuccessView = false;
                }
                else
                {
                    // item not found in incoming detail
                    ErrorText = "Item not found in this P.O";
                    MultipleRowVisible = true;
                    ErrorView = true;
                    SuccessView = false;
                }
            }
            else
            {
                // item not found in item master
                ErrorText = "Item not found in masterlist";
                ErrorView = true;
                MultipleRowVisible = true;
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
