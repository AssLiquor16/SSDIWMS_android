using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
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
        IMainTransactionSync mainTransactionSync;
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        IToastNotifService notifService;
        IncomingHeaderModel _selectedHeader;
        bool _isRefreshing, _genBatchVisible;


        public IncomingHeaderModel SelectedHeader { get => _selectedHeader; set => SetProperty(ref _selectedHeader, value); }
        public bool GenBatchVisible { get => _genBatchVisible; set => SetProperty(ref _genBatchVisible, value); }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public ObservableRangeCollection<IncomingHeaderModel> IncomingHeaderList { get; set; }

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

            GenBactchCodeNavCommand = new AsyncCommand(GenBactchCodeNav);
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            ;

            MessagingCenter.Subscribe<OverviewDetailPopupVM>(this, "ColviewRefreshRetOnGoing", async (page) =>
            {
                await ColViewRefresh();
                await notifService.LoadingProcess("End");
            });
            MessagingCenter.Subscribe<OverviewDetailPopupVM>(this, "ColviewRefresh", async (page) =>
            {
                await ColViewRefresh();
                await notifService.LoadingProcess("End");
            });
            MessagingCenter.Subscribe<BatchGenPOListPopupVM>(this, "RefreshIncomingHeaderList", async (page) =>
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
                        break;
                    case "Pick":
                        var pickerContents = listItems.Where(x => x.INCstatus == "Finalized" || x.INCstatus == "Recieved").ToList();
                        pickerContents = pickerContents.Where(x => x.BatchCode == string.Empty || x.BatchCode == null).ToList();
                        IncomingHeaderList.Clear();
                        IncomingHeaderList.AddRange(pickerContents);
                        break;
                    default: break;
                }
                await notifService.LoadingProcess("End");
            });
        }
        private async Task GenBactchCodeNav() => await PopupNavigation.Instance.PushAsync(new BatchGenPOListPopupPage());    
        //await Shell.Current.GoToAsync($"{nameof(BatchHeaderListPage)}"); 
        private async Task Tapped()
        {
            if(SelectedHeader != null)
            {
                var filter = Preferences.Get("PrefUserRole", "");
                Preferences.Set("PrefPONumber", SelectedHeader.PONumber);
                Preferences.Set("PrefBillDoc", SelectedHeader.BillDoc);
                Preferences.Set("PrefCvan", SelectedHeader.CVAN);
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
            var listItems = await localDbIncomingHeaderService.GetList("WhId,CurDate,OngoingIncStat", null, null, null);
            var filter = Preferences.Get("PrefUserRole", "");
            IsRefreshing = false;
            switch (filter)
            {
                case "Check":
                    var checkerContents = listItems.Where(x => x.INCstatus == "Ongoing").ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(checkerContents);
                    break;
                case "Pick":
                    var pickerContents = listItems.Where(x => x.INCstatus == "Finalized"|| x.INCstatus == "Recieved").ToList();
                    pickerContents = pickerContents.Where(x => x.BatchCode == string.Empty || x.BatchCode == null).ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(pickerContents);
                    break;
                default: break;
            }
        }
        private async Task PageRefresh()
        {

            if(Preferences.Get("PrefIncomingHeaderPagepartialRefresh", false) == false)
            {
                await LiveTimer();
                var listItems = await localDbIncomingHeaderService.GetList("WhId,CurDate,OngoingIncStat", null, null, null);
                var filter = Preferences.Get("PrefUserRole", "");
                switch (filter)
                {
                    case "Check":
                        var checkerContents = listItems.Where(x => x.INCstatus == "Ongoing").ToList();
                        IncomingHeaderList.Clear();
                        IncomingHeaderList.AddRange(checkerContents);
                        GenBatchVisible = false;
                        break;
                    case "Pick":
                        var pickerContents = listItems.Where(x => x.INCstatus == "Finalized" || x.INCstatus == "Recieved").ToList();
                        pickerContents = pickerContents.Where(x => x.BatchCode == string.Empty || x.BatchCode == null).ToList();
                        IncomingHeaderList.Clear();
                        IncomingHeaderList.AddRange(pickerContents);
                        GenBatchVisible = true;
                        break;
                    default: GenBatchVisible = false; break;
                }
                var userfullname = Preferences.Get("PrefUserFullname", "");
                var name = userfullname.Split(' ');
                UserFullName = name[0];
                Preferences.Set("PrefIncomingHeaderPagepartialRefresh", true);
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
