using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.SMTransactions;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs.BatchPopupVMs;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.BatchGeneratePages.BatchPopupPages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs
{
    public class IncomingHeaderVM : ViewModelBase
    {
        public LiveTime liveTime { get; } = new LiveTime();
        IMainTransactionSync mainTransactionSync;
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        IToastNotifService notifService;




        DummyIncomingHeaderModel _selectedHeader;
        bool _isRefreshing, _genBatchVisible, _summaryBtnVisible, _isShowConsolidation;
        public DummyIncomingHeaderModel SelectedHeader { get => _selectedHeader; set => SetProperty(ref _selectedHeader, value); }
        public bool GenBatchVisible { get => _genBatchVisible; set => SetProperty(ref _genBatchVisible, value); }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public bool SummaryBtnVisible { get => _summaryBtnVisible; set => SetProperty(ref _summaryBtnVisible, value); }
        public ObservableRangeCollection<IncomingHeaderModel> IncomingHeaderList { get; set; }
        public ObservableRangeCollection<DummyIncomingHeaderModel> DummyIncomingHeaderList { get; set; }

        public bool IsShowConsolidation
        {
            get => _isShowConsolidation;
            set
            {
                if (value == _isShowConsolidation)
                    return;
                _isShowConsolidation = value;
                foreach(var a in DummyIncomingHeaderList)
                {
                    a.IsConsoLidation = value;
                }
                OnPropertyChanged();
            }
        }

        public AsyncCommand NavToSummaryCommand { get; }
        public AsyncCommand GenBactchCodeNavCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }

        public IncomingHeaderVM()
        {
            mainTransactionSync = DependencyService.Get<IMainTransactionSync>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            notifService = DependencyService.Get<IToastNotifService>();

            IncomingHeaderList = new ObservableRangeCollection<IncomingHeaderModel>();
            DummyIncomingHeaderList = new ObservableRangeCollection<DummyIncomingHeaderModel>();

            NavToSummaryCommand = new AsyncCommand(NavToSummary);
            GenBactchCodeNavCommand = new AsyncCommand(GenBactchCodeNav);
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            MessagingCenter.Subscribe<OverviewDetailPopupVM>(this, "ColviewRefreshRetOnGoing", async (page) =>
            {
                await ViewChanger();
                await notifService.LoadingProcess("End");
            });
            MessagingCenter.Subscribe<OverviewDetailPopupVM>(this, "ColviewRefresh", async (page) =>
            {
                await ViewChanger();
                await notifService.LoadingProcess("End");
            });
            MessagingCenter.Subscribe<BatchGenPOListPopupVM>(this, "RefreshIncomingHeaderList", async (page) =>
            {
                await ViewChanger();
                await notifService.LoadingProcess("End");
            });
        }
        private async Task GenBactchCodeNav() => await PopupNavigation.Instance.PushAsync(new BatchGenPOListPopupPage());
        private async Task NavToSummary() => await Shell.Current.GoToAsync($"{nameof(SummaryPage)}");
        //await Shell.Current.GoToAsync($"{nameof(BatchHeaderListPage)}"); 
        private async Task Tapped()
        {
            if (SelectedHeader != null)
            {
                var filter = Preferences.Get("PrefUserRole", "");
                Preferences.Set("PrefPONumber", SelectedHeader.PONumber);
                Preferences.Set("PrefBillDoc", SelectedHeader.BillDoc);
                Preferences.Set("PrefCvan", SelectedHeader.CVan);
                Preferences.Set("PrefShipNo", SelectedHeader.ShipNo);
                Preferences.Set("PrefShipLine", SelectedHeader.ShipLine);
                switch (filter)
                {
                    case "Check":
                        var route = $"{nameof(IncomingDetailListPage)}";
                        await Shell.Current.GoToAsync(route);
                        break;
                    case "Pick":
                        if (SelectedHeader.INCstatus == "Finalized")
                        {
                            var route1 = $"{nameof(IncomingDetailListPage)}";
                            await Shell.Current.GoToAsync(route1);
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new RecievedOverViewDetailsPopupPage());
                        }
                        break;
                }


            }
            SelectedHeader = null;
        }
        private async Task ColViewRefresh()
        {
            IncomingHeaderList.Clear();
            await Sync();
            await ViewChanger();
        }
        private async Task PageRefresh()
        {

            if (Preferences.Get("PrefIncomingHeaderPagepartialRefresh", false) == false)
            {
                await liveTime.LiveTimer();
                await ViewChanger();
                var userfullname = Preferences.Get("PrefUserFullname", "");
                Preferences.Set("PrefIncomingHeaderPagepartialRefresh", true);
            }

        }
        private async Task ViewChanger()
        {
            IncomingHeaderList.Clear();
            var listItems = await localDbIncomingHeaderService.GetList("WhId,CurDate,OngoingIncStat", null, null, null);
            var filter = Preferences.Get("PrefUserRole", "");
            IsRefreshing = false;
            switch (filter)
            {
                case "Check":
                    var checkerContents = listItems.Where(x => x.INCstatus == "Ongoing").ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(checkerContents);
                    GenBatchVisible = false;
                    SummaryBtnVisible = false;
                    DummyIncomingHeaderList.Clear();
                    foreach(var c in IncomingHeaderList)
                    {
                        DummyIncomingHeaderList.Add(new DummyIncomingHeaderModel
                        {
                            INCId = c.INCId,
                            PONumber = c.PONumber,
                            BillDoc = c.BillDoc,
                            CVan = c.CVAN,
                            ShipNo = c.ShipNo,
                            ShipLine = c.ShipLine,
                            INCstatus = c.INCstatus,
                            ShipCode = c.ShipCode,
                            IsConsoLidation = false,
                            IsSelected = false
                            
                        });
    }
                    break;
                case "Pick":
                    var pickerContents = listItems.Where(x => x.INCstatus == "Finalized" || x.INCstatus == "Recieved").ToList();
                    pickerContents = pickerContents.Where(x => x.BatchCode == string.Empty || x.BatchCode == null).ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(pickerContents);
                    foreach(var p in IncomingHeaderList)
                    {
                        DummyIncomingHeaderList.Add(new DummyIncomingHeaderModel
                        {
                            INCId = p.INCId,
                            PONumber = p.PONumber,
                            BillDoc = p.BillDoc,
                            CVan = p.CVAN,
                            ShipNo = p.ShipNo,
                            ShipLine = p.ShipLine,
                            INCstatus = p.INCstatus,
                            ShipCode = p.ShipCode,
                            IsConsoLidation = false,
                            IsSelected = false
                        });
                    }
                    GenBatchVisible = true;
                    break;
                default: break;
            }
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
