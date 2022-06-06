using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
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
    [QueryProperty(nameof(DataSender), nameof(DataSender))]
    [QueryProperty(nameof(INCParDetId), nameof(INCParDetId))]
    [QueryProperty(nameof(RefId), nameof(RefId))]
    public class EditDetailModuleVM : ViewModelBase
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        IToastNotifService notifyService;
        IncomingPartialDetailModel _e;
        DateTime _expiryDate;
        string _refId, _poNumber, _itemCode, _itemDesc, _palletCode, _amount, _dataSender, _enableColor, _warehouseLocation;
        int _incParDetId, _partialCQTY;
        bool _palleteditEnable, _expiryDateEditEnable, _parQtyEditEnable, _locationVisible;

        public int INCParDetId { get => _incParDetId; set => SetProperty(ref _incParDetId, value); }
        public string RefId { get => _refId; set => SetProperty(ref _refId, value); }

        public IncomingPartialDetailModel E { get => _e; set => SetProperty(ref _e, value); }
        public string DataSender { get => _dataSender; set => SetProperty(ref _dataSender, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string ItemCode { get => _itemCode; set => SetProperty(ref _itemCode, value); }
        public string ItemDesc { get => _itemDesc; set => SetProperty(ref _itemDesc, value); }
        public string Amount { get => _amount; set => SetProperty(ref _amount, value); }
        public string WarehouseLocation { get => _warehouseLocation; set => SetProperty(ref _warehouseLocation, value); }
        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }
        public DateTime ExpiryDate { get => _expiryDate; set => SetProperty(ref _expiryDate, value); }
        public int PartialCQTY { get => _partialCQTY; set => SetProperty(ref _partialCQTY, value); }

        public string EnableColor { get => _enableColor; set => SetProperty(ref _enableColor, value); }
        public bool PalleteditEnable { get => _palleteditEnable; set => SetProperty(ref _palleteditEnable, value); }
        public bool ExpiryDateEditEnable { get => _expiryDateEditEnable; set => SetProperty(ref _expiryDateEditEnable, value); }
        public bool ParQtyEditEnable { get => _parQtyEditEnable; set => SetProperty(ref _parQtyEditEnable, value); }
        public bool LocationVisible { get => _locationVisible; set => SetProperty(ref _locationVisible, value); }

        public AsyncCommand NavLocationListCommand { get; }
        public AsyncCommand NavPalletListCommand { get; }
        public AsyncCommand EditDetailCommand { get; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }

        public EditDetailModuleVM()
        {
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            notifyService = DependencyService.Get<IToastNotifService>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            CancelCommand = new AsyncCommand(Cancel);
            NavLocationListCommand = new AsyncCommand(NavLocationList);
            NavPalletListCommand = new AsyncCommand(NavPalletList);
            EditDetailCommand = new AsyncCommand(EditDetail);

            MessagingCenter.Subscribe<PalletMasterListDetailSubModuleVM, string>(this, "FromPalletListToEdit", (page, e) =>
            {
                PalletCode = e;
            });
        }

        private async Task NavLocationList() => await Shell.Current.GoToAsync($"{nameof(PalletMasterListDetailSubModulePage)}?PageCameFrom=EditDetail");


        private async Task PageRefresh()
        {
            await LiveTimer();
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
            var userRole = Preferences.Get("PrefUserRole", string.Empty);
            switch (userRole)
            {
                case "Check":
                    PalleteditEnable = true;
                    ExpiryDateEditEnable = true;
                    ParQtyEditEnable = true;
                    EnableColor = "#ffdbb3";
                    LocationVisible = false;
                    break;
                case "Pick":
                    PalleteditEnable = true;
                    ExpiryDateEditEnable = true;
                    ParQtyEditEnable = false;
                    EnableColor = "#f2f2f0";
                    LocationVisible = true;
                    break;
                default:
                    PalleteditEnable = false;
                    ExpiryDateEditEnable = false;
                    ParQtyEditEnable = false;
                    EnableColor = "#f2f2f0";
                    LocationVisible = false;
                    break;
            }
            if (!string.IsNullOrWhiteSpace(DataSender))
            {
                DateTime dCreated = Preferences.Get("PrefINCParDetDateCreated", DateTime.MinValue);
                int[] i = { INCParDetId };
                string[] s = { RefId };
                DateTime[] t = { dCreated };
                var retdata = await localDbIncomingParDetailService.GetModel("INCParDetId&RefId&DateCreated", s, i, t);
                if (retdata != null)
                {
                    E = retdata;
                    PONumber = retdata.POHeaderNumber;
                    ItemCode = retdata.ItemCode;
                    ItemDesc = retdata.ItemDesc;
                    PalletCode = retdata.PalletCode;
                    ExpiryDate = retdata.ExpiryDate;
                    PartialCQTY = retdata.PartialCQTY;

                }
            }

        }
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
        private async Task NavPalletList()
        {
            var route = $"{nameof(PalletMasterListDetailSubModulePage)}?PageCameFrom=EditDetail";
            await Shell.Current.GoToAsync(route);
        }
        private async Task EditDetail()
        {
            if (E != null)
            {
                bool isProceed = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to edit the item?", "Yes", "No");
                if(isProceed == true)
                {
                    if (Preferences.Get("PrefUserRole", string.Empty) == "Check")
                    {
                        if (!string.IsNullOrWhiteSpace(PalletCode))
                        {
                            E.TimesUpdated++;
                            E.PalletCode = PalletCode;
                            E.PartialCQTY = PartialCQTY;
                            E.ExpiryDate = ExpiryDate;
                            await localDbIncomingParDetailService.Update("RefId&DateCreated", E);
                            await notifyService.StaticToastNotif("Success", "Detail updated succesfully");
                            MessagingCenter.Send(this, "FromDetailsEditMSG", "EditRefresh");
                            await Shell.Current.GoToAsync("..");
                        }
                    }
                    else if(Preferences.Get("PrefUserRole", string.Empty) == "Pick")
                    {
                        if (!string.IsNullOrWhiteSpace(PalletCode) || !string.IsNullOrWhiteSpace(WarehouseLocation))
                        {
                            E.TimesUpdated++;
                            E.PalletCode = PalletCode;
                            E.PartialCQTY = PartialCQTY;
                            E.ExpiryDate = ExpiryDate;
                            await localDbIncomingParDetailService.Update("RefId&DateCreated", E);
                            await notifyService.StaticToastNotif("Success", "Detail updated succesfully");
                            MessagingCenter.Send(this, "FromDetailsEditMSG", "EditRefresh");
                            await Shell.Current.GoToAsync("..");
                        }
                    }
                }

            }else
            {
                await notifyService.StaticToastNotif("Error", "Passed data is null.");
                await Shell.Current.GoToAsync("..");
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
