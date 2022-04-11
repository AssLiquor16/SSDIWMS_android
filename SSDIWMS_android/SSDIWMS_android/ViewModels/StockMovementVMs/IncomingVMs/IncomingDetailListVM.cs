using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingSubPages;
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

        bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }
        public AsyncCommand AddNavigationCommand { get;  }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public IncomingDetailListVM()
        {
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();

            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();

            AddNavigationCommand = new AsyncCommand(AddNavigation);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            IncomingDetailList.Clear();
            var detailList = await localDbIncomingDetailService.GetList("PONumber,TimesUpdated", null, null);
            IncomingDetailList.AddRange(detailList);
            await LiveTimer();
            UserFullName = Preferences.Get("PrefUserFullname", "");
            PONumber = Preferences.Get("PrefPONumber", "");
        }
        private async Task AddNavigation()
        {
            var route = $"{nameof(IncomingDetailAddPage)}";
            await Shell.Current.GoToAsync(route);
        }
        private async Task ColViewRefresh()
        {
            IsRefreshing = true;

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
