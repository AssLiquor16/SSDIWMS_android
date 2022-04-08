using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader;
using SSDIWMS_android.Services.NotificationServices;
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
        ISMSIncomingHeaderServices serverDbIncomingHeaderService;
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
            serverDbIncomingHeaderService = DependencyService.Get<ISMSIncomingHeaderServices>();
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
                await notifService.StaticToastNotif("Success", "You tapped : " + SelectedHeader.CstmrName);
                SelectedHeader = null;
            }
        }
        private async Task ColViewRefresh()
        {
            IsRefreshing = true;

            IncomingHeaderList.Clear();
            try
            {
                var content = await serverDbIncomingHeaderService.GetList("All", null, null, null);
                foreach (var item in content)
                {
                    int[] intarray = { item.INCId };
                    var ret = await localDbIncomingHeaderService.GetModel("INCId", null, intarray, null);
                    if (ret == null)
                    {
                        await localDbIncomingHeaderService.Insert("Common", item);
                    }
                    else
                    {
                        await localDbIncomingHeaderService.Update("Common", item);
                    }
                }
                await notifService.StaticToastNotif("Success", "Header updated succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", "Cannot connect to server.");
            }
            var listItems = await localDbIncomingHeaderService.GetList("WhId,CurDate,OngoingIncStat", null, null, null);

            var filter = Preferences.Get("PrefUserRole", "");
            switch (filter)
            {
                case "Check":
                    var checkerContents = listItems.Where(x => x.INCstatus == "Ongoing").ToList();
                    IncomingHeaderList.AddRange(checkerContents);
                    break;
                case "Pick":
                    var pickerContents = listItems.Where(x => x.INCstatus == "Finalized").ToList();
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
                    var checkerContents = listItems.Where(x => x.INCstatus == "Ongoing").ToList();
                    IncomingHeaderList.AddRange(checkerContents);
                    break;
                case "Pick":
                    var pickerContents = listItems.Where(x => x.INCstatus == "Finalized").ToList();
                    IncomingHeaderList.AddRange(pickerContents);
                    break;
                default: break;
            }

        }
        

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt");
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
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
