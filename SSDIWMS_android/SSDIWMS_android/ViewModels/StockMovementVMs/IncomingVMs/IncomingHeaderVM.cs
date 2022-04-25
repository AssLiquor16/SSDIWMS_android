using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages;
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
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        IUpdateAllIncomingtransaction transactionUpdateService;
        IToastNotifService notifService;

        IncomingHeaderModel _selectedHeader;
        bool _isRefreshing;


        public IncomingHeaderModel SelectedHeader { get => _selectedHeader; set => SetProperty(ref _selectedHeader, value); }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public ObservableRangeCollection<IncomingHeaderModel> IncomingHeaderList { get; set; }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }

        public IncomingHeaderVM()
        {
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            transactionUpdateService = DependencyService.Get<IUpdateAllIncomingtransaction>();
            notifService = DependencyService.Get<IToastNotifService>();

            IncomingHeaderList = new ObservableRangeCollection<IncomingHeaderModel>();

            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

        }
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
                        var route1 = $"{nameof(IncomingDetailListPage)}";
                        await Shell.Current.GoToAsync(route1);
                        break;
                }

                
            }
            SelectedHeader = null;
        }
        private async Task ColViewRefresh()
        {
            IsRefreshing = true;
            IncomingHeaderList.Clear();
            var syncing = Preferences.Get("PrefISMSyncing", false);
            if (syncing == false)
            {
                Preferences.Set("PrefISMSyncing", true);
                try
                {
                    await transactionUpdateService.UpdateAllIncomingTrans();
                    await notifService.StaticToastNotif("Success", "Items updated succesfully.");
                }
                catch
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
                
                Preferences.Set("PrefISMSyncing", false);
            }
            else
            {
                await notifService.StaticToastNotif("Error", "Syncing busy, please try again later.");
            }
            var listItems = await localDbIncomingHeaderService.GetList("WhId,CurDate,OngoingIncStat", null, null, null);
            var filter = Preferences.Get("PrefUserRole", "");
            switch (filter)
            {
                case "Check":
                    var checkerContents = listItems.Where(x => x.INCstatus == "Ongoing" || x.INCstatus == "Finalized").ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(checkerContents);
                    break;
                case "Pick":
                    var pickerContents = listItems.Where(x => x.INCstatus == "Finalized").ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(pickerContents);
                    break;
                default: break;
            }
            IsRefreshing = false;

        }
        private async Task PageRefresh()
        {
            await LiveTimer();
            IncomingHeaderList.Clear();
            var listItems = await localDbIncomingHeaderService.GetList("WhId,CurDate,OngoingIncStat", null, null, null);
            var filter = Preferences.Get("PrefUserRole", "");
            switch (filter)
            {
                case "Check":
                    var checkerContents = listItems.Where(x => x.INCstatus == "Ongoing" || x.INCstatus == "Finalized").ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(checkerContents);
                    break;
                case "Pick":
                    var pickerContents = listItems.Where(x => x.INCstatus == "Finalized").ToList();
                    IncomingHeaderList.Clear();
                    IncomingHeaderList.AddRange(pickerContents);
                    break;
                default: break;
            }
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
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
