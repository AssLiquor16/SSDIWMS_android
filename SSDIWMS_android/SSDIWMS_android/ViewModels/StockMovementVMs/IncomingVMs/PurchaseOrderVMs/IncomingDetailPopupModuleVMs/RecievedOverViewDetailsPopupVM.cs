using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.UserMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.PurchaseOrderPages.IncomingDetailPopupModulePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs
{
    public class RecievedOverViewDetailsPopupVM : ViewModelBase
    {
        IMainServices mainServices;
        ILocalUserServices localDbUserService;
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        ISMSIncomingHeaderServices serverDbIncomingHeaderService;

        public LiveTime liveTime { get; } = new LiveTime();
        string _userRole, _billDOc, _cVan, _shipNo, _shipline, _userFullName;
        DateTime _dateRec;
        IncomingDetailModel _selectedItem;
        string _poNumber, _totalPOItems, _buttonText;
        bool _isrefreshing, _returnView;
        public IncomingDetailModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string TotalPOItems { get => _totalPOItems; set => SetProperty(ref _totalPOItems, value); }
        public bool IsRefreshing { get => _isrefreshing; set => SetProperty(ref _isrefreshing, value); }
        public bool ReturnView { get => _returnView; set => SetProperty(ref _returnView, value); }

        public string UserRole { get => _userRole; set => SetProperty(ref _userRole, value); }
        public string UserFullName { get => _userFullName; set => SetProperty(ref _userFullName, value); }
        public string ButtonText { get => _buttonText; set => SetProperty(ref _buttonText, value); }
        public string BillDoc { get => _billDOc; set => SetProperty(ref _billDOc, value); }
        public string CVAN { get => _cVan; set => SetProperty(ref _cVan, value); }
        public string ShipNo { get => _shipNo; set => SetProperty(ref _shipNo, value); }
        public string ShippingLine { get => _shipline; set => SetProperty(ref _shipline, value); }
        public DateTime DateRec { get => _dateRec; set => SetProperty(ref _dateRec, value); }
        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public RecievedOverViewDetailsPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbUserService = DependencyService.Get<ILocalUserServices>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            serverDbIncomingHeaderService = DependencyService.Get<ISMSIncomingHeaderServices>();
            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();
            CancelCommand = new AsyncCommand(Cancel);
            TappedCommand = new AsyncCommand(Tapped);
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
                await PopupNavigation.Instance.PushAsync(new PartialDetListPopupPage(SelectedItem));
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
            PONumber = Preferences.Get("PrefPONumber", string.Empty);
            BillDoc = Preferences.Get("PrefBillDoc", string.Empty);
            CVAN = Preferences.Get("PrefCvan", string.Empty);
            ShipNo = Preferences.Get("PrefShipNo", string.Empty);
            ShippingLine = Preferences.Get("PrefShipLine", string.Empty);
            await QueryAll();
        }
        public async Task QueryAll()
        {
            IncomingDetailList.Clear();

            var totalpartialcqty = 0;
            switch(Preferences.Get("PrefUserRole", string.Empty))
            {
                case "Pick":
                    string[] filter =
                    {
                        Preferences.Get("PrefPONumber", string.Empty)
                    };
                    UserRole = "Rec Date";
                    var a = await localDbIncomingHeaderService.GetModel("PONumber", filter, null,null);
                    DateRec = a.RecDate;
                    break;
                default: break;


            }
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
            }
            IncomingDetailList.AddRange(groupedAllItemInThisPo);
            var n = IncomingDetailList.Count;
            TotalPOItems = n + " " + "Items";


        }
    }
}
