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
            var groupedAllItemInThisPo = AllItemInThisPo.GroupBy(l => l.ItemCode).Select(cl => new IncomingDetailModel
            {
                INCDetId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().INCDetId,
                INCHeaderId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().INCHeaderId,
                ItemCode = cl.Key,
                ItemDesc = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                Qty = cl.Sum(x => x.Qty),
                UserId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().UserId,
                Amount = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().Amount,
                TimesUpdated = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().TimesUpdated,
                POHeaderNumber = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().POHeaderNumber,
                DateSync = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().DateSync,
            }).ToList();
            foreach (var item in groupedAllItemInThisPo)
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
            IncomingDetailList.AddRange(groupedAllItemInThisPo);
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
                                await notifService.StaticToastNotif("Success", "PO finalized.");
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
                            await notifService.StaticToastNotif("Success", "PO Recieved.");
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
                            try
                            {
                                await Sync();
                                await notifService.StaticToastNotif("Success", "PO Recieved.");
                            }
                            catch
                            {
                                await notifService.StaticToastNotif("Error", "Cannot connect to server");
                            }
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
                        try
                        {
                            await Sync();
                            await notifService.StaticToastNotif("Success", "PO Recieved.");
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
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "User role not found.", "Ok");
                await mainServices.RemovePreferences();
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
            await notifService.LoadingProcess("End");
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
                    var incdets = await localDbIncomingDetailService.GetList("PONumber2", new string[] { Preferences.Get("PrefPONumber", string.Empty) }, null);
                    List<IncomingDetailModel> incdetlist = new List<IncomingDetailModel>();
                    var multskuiCode = string.Empty;
                    var rowpardet1 = 0;
                    incdetlist.Clear();
                    foreach (var incdet in incdets)
                    {

                        var multcheck = incdets.Where(x => x.ItemCode == incdet.ItemCode).ToList();
                        if(multcheck.Count() == 1)
                        {
                        var rowcount1pardet = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", new string[] { incdet.ItemCode }, new int[] { incdet.INCDetId });
                        foreach (var incpardet in rowcount1pardet)
                        {
                            rowpardet1 += incpardet.PartialCQTY;
                        }
                            multcheck[0].TimesUpdated += 2;
                            multcheck[0].UserId = userId;
                            multcheck[0].QTYStatus = string.Empty;
                            multcheck[0].DateSync = DateTime.Now;
                            multcheck[0].Cqty = rowpardet1;
                            await localDbIncomingDetailService.Update("Common", multcheck[0]);
                            rowpardet1 = 0;
                            foreach (var incpardet in rowcount1pardet)
                            {
                            incpardet.TimesUpdated += 2;
                            incpardet.UserId = userId;
                            incpardet.Status = "Finalized";
                            incpardet.DateFinalized = DateTime.Now;
                            incpardet.DateSync = DateTime.Now;
                            await localDbIncomingParDetailService.Update("RefId&DateCreated", incpardet);
                            }
                        }
                        else if(multcheck.Count() > 1)
                        {
                            foreach(var mltcheck in multcheck)
                        {
                            var checkifexist = incdetlist.Where(x => x.INCDetId == mltcheck.INCDetId).FirstOrDefault();
                            if(checkifexist == null)
                            {
                                incdetlist.Add(mltcheck);
                            }
                        }
                        }
                    }
                    var l = 0;
                    var row = 0;
                    var queried = false;
                    foreach(var indetlist in incdetlist)
                    {
                    if (queried == false)
                    {
                        l = IncomingDetailList.Where(x => x.ItemCode == indetlist.ItemCode).FirstOrDefault().Cqty;
                        queried = true;
                    }
                    row++;
                    indetlist.TimesUpdated += 2;
                    indetlist.UserId = userId;
                    indetlist.QTYStatus = string.Empty;
                    indetlist.DateSync = DateTime.Now;
                    if (l >= indetlist.Qty)
                    {
                        if (row != incdetlist.Count)
                        {
                            indetlist.Cqty = indetlist.Qty;
                            l = l - indetlist.Qty;
                        }
                        else
                        {
                            indetlist.Cqty = l;
                            l = l - l;
                        }
                    }
                    else
                    {
                        indetlist.Cqty = l;
                        l = l - l;
                    }
                    await localDbIncomingDetailService.Update("Common", indetlist);
                    var pardetspersku = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", new string[] { indetlist.ItemCode }, new int[] { indetlist.INCDetId });
                    foreach(var pardet in pardetspersku)
                    {
                        pardet.TimesUpdated += 2;
                        pardet.UserId = userId;
                        pardet.Status = "Finalized";
                        pardet.DateFinalized = DateTime.Now;
                        pardet.DateSync = DateTime.Now;
                        await localDbIncomingParDetailService.Update("RefId&DateCreated", pardet);
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
                    var incdets = await localDbIncomingDetailService.GetList("PONumber2", new string[] { Preferences.Get("PrefPONumber", string.Empty) }, null);
                foreach (var item in incdets)
                {
                    item.TimesUpdated += 20;
                    item.QTYStatus = string.Empty;
                    item.DateSync = DateTime.Now;
                    await localDbIncomingDetailService.Update("Common", item);
                    var retpardet = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", new string[] { item.ItemCode }, new int[] { item.INCDetId } );
                        foreach (var paritem in retpardet)
                        {
                            paritem.TimesUpdated += 20;
                            paritem.Status = "Recieved";
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
                    var incdets = await localDbIncomingDetailService.GetList("PONumber2", new string[] { Preferences.Get("PrefPONumber", string.Empty) }, null);
                    foreach (var item in incdets)
                    {
                        item.TimesUpdated += 15;
                        item.QTYStatus = string.Empty;
                        item.DateSync = DateTime.Now;
                        await localDbIncomingDetailService.Update("Common", item);
                        var retpardet = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", new string[] { item.ItemCode }, new int[] {item.INCDetId});
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
                    await notifService.StaticToastNotif("Success", "PO returned.");
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
            await notifService.LoadingProcess("End");
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

