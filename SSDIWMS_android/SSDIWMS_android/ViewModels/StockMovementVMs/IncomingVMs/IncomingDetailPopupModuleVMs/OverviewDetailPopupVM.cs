using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.SMTransactions;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages.IncomingDetailSubPopupModulePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs
{
    public class OverviewDetailPopupVM : ViewModelBase
    {
        IMainTransactionSync mainTransactionSync;
        IMainServices mainServices;
        IToastNotifService notifService;
        IUpdateAllIncomingtransaction updaterservice;
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        ISMSIncomingHeaderServices serverDbIncomingHeaderService;

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt");
        string _userFullname, _userRole, _billDOc, _cVan, _shipNo, _shipline;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        IncomingDetailModel _selectedItem;
        string _poNumber, _totalPOItems, _buttonText;
        bool _isrefreshing, _returnView,_showQTY;
        public IncomingDetailModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string TotalPOItems { get => _totalPOItems; set => SetProperty(ref _totalPOItems, value); }
        public bool IsRefreshing { get => _isrefreshing; set => SetProperty(ref _isrefreshing, value); }
        public bool ReturnView { get => _returnView; set => SetProperty(ref _returnView, value); }
        public bool ShowQTY { get => _showQTY; set => SetProperty(ref _showQTY, value); }

        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
        public string UserRole { get => _userRole; set => SetProperty(ref _userRole, value); }
        public string ButtonText { get => _buttonText; set => SetProperty(ref _buttonText, value); }
        public string BillDoc { get => _billDOc; set => SetProperty(ref _billDOc, value); }
        public string CVAN { get => _cVan; set => SetProperty(ref _cVan, value); }
        public string ShipNo { get => _shipNo; set => SetProperty(ref _shipNo, value); }
        public string ShippingLine { get => _shipline; set => SetProperty(ref _shipline, value); }
        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand FinalizeCommand { get; }
        public AsyncCommand ReturnToOngoingCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public OverviewDetailPopupVM()
        {
            mainTransactionSync = DependencyService.Get<IMainTransactionSync>();
            mainServices = DependencyService.Get<IMainServices>();
            notifService = DependencyService.Get<IToastNotifService>();
            updaterservice = DependencyService.Get<IUpdateAllIncomingtransaction>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            serverDbIncomingHeaderService = DependencyService.Get<ISMSIncomingHeaderServices>();
            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();
            CancelCommand = new AsyncCommand(Cancel);
            TappedCommand = new AsyncCommand(Tapped);
            ReturnToOngoingCommand = new AsyncCommand(ReturnToOngoing);
            FinalizeCommand = new AsyncCommand(SetStatus);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Cancel()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async Task Tapped()
        {
            if (SelectedItem != null)
            {
                await PopupNavigation.Instance.PushAsync(new PartialDetailListPopupPage(SelectedItem));
                SelectedItem = null;
            }
        }
        private async Task ColViewRefresh()
        {
            IsRefreshing = true;
            await QueryAll();
            IsRefreshing = false;
        }
        public async Task PageRefresh()
        {
            var userRole = Preferences.Get("PrefUserRole", string.Empty);
            switch (userRole)
            {
                case "Check":
                    UserRole = "Checker";
                    ButtonText = "Finalize";
                    ReturnView = false;
                    break;
                case "Pick":
                    UserRole = "Picker";
                    ButtonText = "Recieve";
                    ReturnView = true;
                    break;
                default:
                    UserRole = "Unassigned";
                    ButtonText = "Unable";
                    ReturnView = false;
                    break;
            }
            BillDoc = Preferences.Get("PrefBillDoc", string.Empty);
            CVAN = Preferences.Get("PrefCvan", string.Empty);
            ShipNo = Preferences.Get("PrefShipNo", string.Empty);
            ShippingLine = Preferences.Get("PrefShipLine", string.Empty);
            UserFullName = Preferences.Get("PrefUserFullname", string.Empty);
            await LiveTimer();
            await QueryAll();
        }
        public async Task QueryAll()
        {
            IncomingDetailList.Clear();
            var totalpartialcqty = 0;
            var AllItemInThisPo = await localDbIncomingDetailService.GetList("PONumber", null, null);
            foreach (var item in AllItemInThisPo)
            {

                int[] g = { item.INCDetId };
                var e = await localDbIncomingParDetailService.GetList("PONumber&INCId", null, g);
                foreach (var ite in e)
                {
                    totalpartialcqty += ite.PartialCQTY;
                }
                if (item.Qty > totalpartialcqty)
                {
                    item.QTYStatus = "Short";
                    item.Color = "Red";
                }
                else if (item.Qty < totalpartialcqty)
                {
                    item.QTYStatus = "Over";
                    item.Color = "Red";
                }
                else if (item.Qty == totalpartialcqty)
                {
                    item.QTYStatus = "Ok";
                    item.Color = "Green";
                }
                item.Cqty = totalpartialcqty;
                totalpartialcqty = 0;
                if (Preferences.Get("PrefUserRole", string.Empty) == "Pick")
                {
                    item.Show = true;
                }
                else
                {
                    item.Show = false;
                }
            }
            IncomingDetailList.AddRange(AllItemInThisPo);
            var n = IncomingDetailList.Count;
            TotalPOItems = n + " " + "Items";


        }
        private async Task SetStatus()
        {
            var role = Preferences.Get("PrefUserRole", string.Empty);
            if (role == "Check")
            {
                //finalize
                if (IncomingDetailList.Where(x => x.QTYStatus == "Short" || x.QTYStatus == "Over").Count() > 0)
                {
                    if (await App.Current.MainPage.DisplayAlert("Alert", "Some SKU have variance.", "Ok", "Cancel") == true)
                    {
                        if (await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to finalize the P.O?", "Yes", "No") == true)
                        {
                            await Finalize();
                            try
                            {
                                await Sync();
                            }
                            catch
                            {
                                await notifService.StaticToastNotif("Error", "Cannot connect to server");
                            }
                            MessagingCenter.Send(this, "ColviewRefresh");
                            await Task.Delay(500);
                            await PopupNavigation.Instance.PopAllAsync(true);
                            var route = $"..";
                            await Shell.Current.GoToAsync(route);
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to recieve the P.O?", "Yes", "No") == true)
                    {
                        await Finalize();
                        try
                        {
                            await Sync();
                        }
                        catch
                        {
                            await notifService.StaticToastNotif("Error", "Cannot connect to server");
                        }
                        MessagingCenter.Send(this, "ColviewRefresh");
                        await Task.Delay(500);
                        await PopupNavigation.Instance.PopAllAsync(true);
                        var route = $"..";
                        await Shell.Current.GoToAsync(route);
                    }
                }

            }
            else if (role == "Pick")
            {
                //recieved
                if (IncomingDetailList.Where(x => x.QTYStatus == "Short" || x.QTYStatus == "Over").Count() > 0)
                {
                    if (await App.Current.MainPage.DisplayAlert("Alert", "Some SKU have variance.", "Ok", "Cancel") == true)
                    {
                        if(await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to finalize the P.O?", "Yes", "No") == true)
                        {
                            await Recieve();
                            MessagingCenter.Send(this, "ColviewRefresh");
                            await Task.Delay(1000);
                            await PopupNavigation.Instance.PopAllAsync(true);
                            var route = $"..";
                            await Shell.Current.GoToAsync(route);
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to finalize the P.O?", "Yes", "No") == true)
                    {
                        await Recieve();
                        MessagingCenter.Send(this, "ColviewRefresh");
                        await Task.Delay(1000);
                        await PopupNavigation.Instance.PopAllAsync(true);
                        var route = $"..";
                        await Shell.Current.GoToAsync(route);
                    }
                }
                
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "User role not found.", "Ok");
                await mainServices.RemovePreferences();
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }
        private async Task Finalize()
        {
                try
                {
                    await notifService.LoadingProcess("Begin", "Attempting to sync...");
                    var userId = Preferences.Get("PrefUserId", 0);
                    IncomingHeaderModel e = new IncomingHeaderModel
                    {
                        FinalUserId = userId,
                        INCstatus = "Finalized",
                        PONumber = PONumber,
                        TimesUpdated = 1,
                        DateSync = DateTime.Now
                        
                    };
                    await localDbIncomingHeaderService.Update("PONumber", e);
                    foreach (var item in IncomingDetailList)
                    {
                        item.TimesUpdated += 2;
                        item.UserId = userId;
                        item.QTYStatus = string.Empty;
                        item.DateSync = DateTime.Now;
                        //totals all the qty of partial details where po and item code is equal to item
                        string[] stringfilter = { item.POHeaderNumber, item.ItemCode };
                        var partialdet = await localDbIncomingParDetailService.GetList("PoIc", stringfilter, null);
                        var totalCqty = 0;
                        foreach (var pardet in partialdet)
                        {
                            totalCqty += pardet.PartialCQTY;
                        }
                        //-----------------------------------------------------------------------------
                        item.Cqty = totalCqty;
                        await localDbIncomingDetailService.Update("Common", item);

                        string[] s = { item.ItemCode };
                        int[] i = { item.INCDetId };

                        var retpardet = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", s, i);
                        foreach (var paritem in retpardet)
                        {
                            paritem.TimesUpdated += 2;
                            paritem.UserId = userId;
                            paritem.Status = "Finalized";
                            paritem.DateFinalized = DateTime.Now;
                            paritem.DateSync = DateTime.Now;
                            await localDbIncomingParDetailService.Update("RefId&DateCreated", paritem);
                        }
                    }
                    Preferences.Remove("PrefPONumber");
                    Preferences.Remove("PrefBillDoc");
                    Preferences.Remove("PrefCvan");
                    Preferences.Remove("PrefShipNo");
                }
                catch
                {
                    await notifService.StaticToastNotif("Error", "Something went wrong");
                }

        }
        private async Task Recieve()
        {
                try
                {
                    await notifService.LoadingProcess("Begin", "Attempting to sync...");
                    var userId = Preferences.Get("PrefUserId", 0);
                    IncomingHeaderModel e = new IncomingHeaderModel
                    {
                        RecUserId = userId,
                        INCstatus = "Recieved",
                        PONumber = PONumber,
                        TimesUpdated = 20,
                        DateSync = DateTime.Now

                    };
                    await localDbIncomingHeaderService.Update("PONumber1", e);
                    foreach (var item in IncomingDetailList)
                    {
                        item.TimesUpdated += 20;
                        item.UserId = userId;
                        item.QTYStatus = string.Empty;
                        item.DateSync = DateTime.Now;
                        //totals all the qty of partial details where po and item code is equal to item
                        string[] stringfilter = { item.POHeaderNumber, item.ItemCode };
                        var partialdet = await localDbIncomingParDetailService.GetList("PoIc", stringfilter, null);
                        var totalCqty = 0;
                        foreach (var pardet in partialdet)
                        {
                            totalCqty += pardet.PartialCQTY;
                        }
                        //-----------------------------------------------------------------------------
                        item.Cqty = totalCqty;
                        await localDbIncomingDetailService.Update("Common", item);

                        string[] s = { item.ItemCode };
                        int[] i = { item.INCDetId };

                        var retpardet = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", s, i);
                        foreach (var paritem in retpardet)
                        {
                            paritem.TimesUpdated += 20;
                            paritem.UserId = userId;
                            paritem.Status = "Recieved";
                            paritem.DateFinalized = DateTime.Now;
                            await localDbIncomingParDetailService.Update("RefId&DateCreated", paritem);
                        }
                    }
                    Preferences.Remove("PrefPONumber");
                    Preferences.Remove("PrefBillDoc");
                    Preferences.Remove("PrefCvan");
                    Preferences.Remove("PrefShipNo");
                }
                catch
                {
                    await notifService.StaticToastNotif("Error", "Something went wrong");
                }


                /*try
                {
                    bool busy = Preferences.Get("PrefISMSyncing", false);
                    if (busy == false)
                    {
                        string[] strinfilter = { PONumber };
                        var poStatus = await serverDbIncomingHeaderService.GetString("ReturnStatus", strinfilter, null);
                        if (poStatus == "Finalized")
                        {
                            await updaterservice.UpdateAllIncomingTrans();
                            await notifService.StaticToastNotif("Success", "Item sync successfully.");
                        }
                    }

                }
                catch
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }*/
                
        }
        private async Task ReturnToOngoing()
        {
            if (await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to return the P.O?", "Yes", "No") == true)
            {
                try
                {
                    await notifService.LoadingProcess("Begin", "Attempting to sync...");
                    var userId = Preferences.Get("PrefUserId", 0);
                    IncomingHeaderModel e = new IncomingHeaderModel
                    {
                        RecUserId = userId,
                        INCstatus = "Ongoing",
                        PONumber = PONumber,
                        TimesUpdated = 15,
                        DateSync = DateTime.Now

                    };
                    await localDbIncomingHeaderService.Update("PONumber1", e);
                    foreach (var item in IncomingDetailList)
                    {
                        item.TimesUpdated += 15;
                        item.UserId = userId;
                        item.QTYStatus = string.Empty;
                        item.DateSync = DateTime.Now;
                        //totals all the qty of partial details where po and item code is equal to item
                        string[] stringfilter = { item.POHeaderNumber, item.ItemCode };
                        var partialdet = await localDbIncomingParDetailService.GetList("PoIc", stringfilter, null);
                        var totalCqty = 0;
                        foreach(var pardet in partialdet)
                        {
                            totalCqty += pardet.PartialCQTY;
                        }
                        //-----------------------------------------------------------------------------
                        item.Cqty = totalCqty;
                        await localDbIncomingDetailService.Update("Common", item);
                        string[] s = { item.ItemCode };
                        int[] i = { item.INCDetId };

                        var retpardet = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", s, i);
                        foreach (var paritem in retpardet)
                        {
                            paritem.TimesUpdated += 15;
                            paritem.UserId = userId;
                            paritem.Status = "Ongoing";
                            paritem.DateFinalized = DateTime.Now;
                            paritem.DateSync = DateTime.Now;
                            await localDbIncomingParDetailService.Update("RefId&DateCreated", paritem);
                        }
                    }
                    Preferences.Remove("PrefPONumber");
                    Preferences.Remove("PrefBillDoc");
                    Preferences.Remove("PrefCvan");
                    Preferences.Remove("PrefShipNo");
                }
                catch
                {
                    await notifService.StaticToastNotif("Error", "Something went wrong");
                }

                try
                {
                    await Sync();
                }
                catch
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server");
                }
                MessagingCenter.Send(this, "ColviewRefreshRetOnGoing");
                await Task.Delay(500);
                await PopupNavigation.Instance.PopAllAsync(true);
                var route = $"..";
                await Shell.Current.GoToAsync(route);
            }
            else
            {

            }
        }
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
        private async Task Sync()
        {
            if (Preferences.Get("PrefISMSyncing", false) == false)
            {
                Preferences.Set("PrefISMSyncing", true);
                try
                {
                    await mainTransactionSync.UpdateAllTransactions("Incoming");
                    await notifService.ToastNotif("Success", "Items updated succesfully.");
                }
                catch
                {
                    await notifService.ToastNotif("Error", "Cannot connect to server.");
                }

                Preferences.Set("PrefISMSyncing", false);
                await Task.Delay(500);
                return;
            }
            else
            {
                await notifService.ToastNotif("Error", "Syncing busy, please try again later.");
            }
        }
    }
}

