using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs
{
    public class IncomingDetailListVM : ViewModelBase
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        IUpdateAllIncomingtransaction transactionUpdateService;
        IToastNotifService notifService;

        string _toolbarColor, _role;
        bool _isRefreshing;
        IncomingPartialDetailModel _selectedDetail;
        public string Role { get => _role; set => SetProperty(ref _role, value); }
        public string ToolbarColor { get => _toolbarColor; set => SetProperty(ref _toolbarColor, value); }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public IncomingPartialDetailModel SelectedDetail { get => _selectedDetail; set => SetProperty(ref _selectedDetail, value); }


        public ObservableRangeCollection<IncomingPartialDetailModel> IncomingParDetailList { get; set; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ShowOverViewCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public IncomingDetailListVM()
        {
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            transactionUpdateService = DependencyService.Get<IUpdateAllIncomingtransaction>();
            notifService = DependencyService.Get<IToastNotifService>();


            IncomingParDetailList = new ObservableRangeCollection<IncomingPartialDetailModel>();
            ShowOverViewCommand = new AsyncCommand(ShowOverView);
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);


            MessagingCenter.Subscribe<EditDetailPopupVM, string>(this, "FromDetailsEditMSG", async (page, e) => 
            {
                await ColViewRefresh();
            });
            MessagingCenter.Subscribe<AddDetailPopupVM, string>(this, "FromDetailsAddMSG", async (page, e) =>
            {
                await ColViewRefresh();
            });

        }
        private async Task Tapped()
        {
            if(SelectedDetail != null)
            {
                await PopupNavigation.Instance.PushAsync(new EditDetailPopupPage(SelectedDetail));
                SelectedDetail = null;
            }
        }
        private async Task ShowOverView()
        {
            await PopupNavigation.Instance.PushAsync(new OverviewDetailPopupPage(PONumber));
        }
        public async Task AddPopupNav(string scannedCode)
        {
            string[] datas = { PONumber, scannedCode };
            await PopupNavigation.Instance.PushAsync(new AddDetailPopupPage(datas));
        }
        public async Task PageRefresh()
        {
            Role = Preferences.Get("PrefUserRole", "");
            IncomingParDetailList.Clear();
            var detailList = await localDbIncomingParDetailService.GetList("PONumber", null, null);
            IncomingParDetailList.AddRange(detailList);
            await LiveTimer();
            UserFullName = Preferences.Get("PrefUserFullname", "");
            PONumber = Preferences.Get("PrefPONumber", "");
        }
        public async Task ColViewRefresh()
        {
            
            IsRefreshing = true;
            IncomingParDetailList.Clear();
            var detailList = await localDbIncomingParDetailService.GetList("PONumber", null, null);
            IncomingParDetailList.Clear();
            IncomingParDetailList.AddRange(detailList);
            IsRefreshing = false;
        }

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt"), _userFullname, _pONumber;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
        public string PONumber { get => _pONumber; set => SetProperty(ref _pONumber, value); }
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
