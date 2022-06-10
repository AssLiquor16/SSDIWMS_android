using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using SSDIWMS_android.ViewModels.PopUpVMs;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailModuleVMs;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs;
using SSDIWMS_android.Views.PopUpPages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailModulePages;
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
    public class IncomingDetailListVM : ViewModelBase
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        IUpdateAllIncomingtransaction transactionUpdateService;
        IToastNotifService notifService;

        string _toolbarColor, _role;
        bool _isRefreshing, _addBtnVisbile;
        IncomingPartialDetailModel _selectedDetail;
        public string Role { get => _role; set => SetProperty(ref _role, value); }
        public string ToolbarColor { get => _toolbarColor; set => SetProperty(ref _toolbarColor, value); }
        public bool AddBtnVisbile { get => _addBtnVisbile; set => SetProperty(ref _addBtnVisbile, value); }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public IncomingPartialDetailModel SelectedDetail { get => _selectedDetail; set => SetProperty(ref _selectedDetail, value); }


        public ObservableRangeCollection<IncomingPartialDetailModel> IncomingParDetailList { get; set; }
        public AsyncCommand SyncCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ShowOverViewCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand NavToAddCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public IncomingDetailListVM()
        {
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            transactionUpdateService = DependencyService.Get<IUpdateAllIncomingtransaction>();
            notifService = DependencyService.Get<IToastNotifService>();


            IncomingParDetailList = new ObservableRangeCollection<IncomingPartialDetailModel>();

            NavToAddCommand = new AsyncCommand(NavToAdd);
            SyncCommand = new AsyncCommand(Sync);
            ShowOverViewCommand = new AsyncCommand(ShowOverView);
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);


            MessagingCenter.Subscribe<EditDetailModuleVM, string>(this, "FromDetailsEditMSG", async (page, e) => 
            {
                await ColViewRefresh();
            });
            MessagingCenter.Subscribe<AddDetailModuleVM, string>(this, "FromDetailsAddMSG", async (page, e) =>
            {
                await ColViewRefresh();
            });



            //rejects
            //SyncCommand = new AsyncCommand(Sync);

        }
        private async Task Tapped()
        {
            if(SelectedDetail != null)
            {

                var route = $"{nameof(EditDetailModulePages)}?DataSender=DetailList&INCParDetId={SelectedDetail.INCParDetId}&RefId={SelectedDetail.RefId}";
                Preferences.Set("PrefINCParDetDateCreated", SelectedDetail.DateCreated);
                Preferences.Remove("PrefIncomingInitialRefresh");
                await Shell.Current.GoToAsync(route);
            }
        }
        private async Task Sync()
        {
            await notifService.LoadingProcess("Begin", "Attempting to sync...");
            var syncing = Preferences.Get("PrefISMSyncing", false);
            if (syncing == false)
            {
                Preferences.Set("PrefISMSyncing", true);
                try
                {
                    await transactionUpdateService.UpdateAllIncomingTrans();
                    await notifService.StaticToastNotif("Success", "Items synced succesfully.");
                }
                catch
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
            }
            else
            {
                await notifService.StaticToastNotif("Error", "Sync busy, try again later.");
            }
            Preferences.Set("PrefISMSyncing", false);
            await Shell.Current.GoToAsync("..");
            await notifService.LoadingProcess("End", " ");

        }
        private async Task ShowOverView()
        {
            await PopupNavigation.Instance.PushAsync(new OverviewDetailPopupPage(PONumber));
        }
        private async Task NavToAdd()
        {
            Preferences.Remove("PrefAddPartialDetail2InitialRefresh");
            await Shell.Current.GoToAsync($"{nameof(AddDetailModule2Page)}");
        }
        public async Task AddPopupNav(string scannedCode)
        {
            /*
             *  string[] datas = { PONumber, scannedCode };
            await PopupNavigation.Instance.PushAsync(new AddDetailPopupPage(datas));
            */
            var route = $"{nameof(AddDetailModulePage)}?PONumber={PONumber}&ScannedCode={scannedCode}";
            await Shell.Current.GoToAsync(route);
        }
        public async Task PageRefresh()
        {
            Role = Preferences.Get("PrefUserRole", "");
            IncomingParDetailList.Clear();
            var detailList = await localDbIncomingParDetailService.GetList("PONumber", null, null);
            if (Role == "Check")
            {
                var checklist = detailList.Where(x => x.Status == "Ongoing" || x.Status == "Finalized").ToList();
                IncomingParDetailList.Clear();
                IncomingParDetailList.AddRange(checklist);
                AddBtnVisbile = true;
            }
            else if (Role == "Pick")
            {
                var checklist = detailList.Where(x => x.Status == "Finalized").ToList();
                IncomingParDetailList.Clear();
                IncomingParDetailList.AddRange(checklist);
                AddBtnVisbile = false;
            }
            else
            {
                IncomingParDetailList.Clear();
                AddBtnVisbile = false;
            }
            await LiveTimer();
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];
            PONumber = Preferences.Get("PrefPONumber", "");
        }
        public async Task ColViewRefresh()
        {
            
            IsRefreshing = true;
            var detailList = await localDbIncomingParDetailService.GetList("PONumber", null, null);
            if (Role == "Check")
            {
                var checklist = detailList.Where(x => x.Status == "Ongoing" || x.Status == "Finalized").ToList();
                IncomingParDetailList.Clear();
                IncomingParDetailList.AddRange(checklist);
                
            }
            else if (Role == "Pick")
            {
                var checklist = detailList.Where(x => x.Status == "Finalized").ToList();
                IncomingParDetailList.Clear();
                IncomingParDetailList.AddRange(checklist);
            }
            else
            {
                IncomingParDetailList.Clear();
            }
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



        //rejects 
        /*private async Task Sync()
        {
            var con = new LoadingPopupVM();
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage(""));
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
            await con.CloseAll();
            await ColViewRefresh();
        }*/
    }
}
