using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.IncomingDetailPopupModulePages.IncomingDetailSubPopupModulePages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs
{
    public class OverviewDetailPopupVM : ViewModelBase
    {
        IMainServices mainServices;
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        
        IncomingDetailModel _selectedItem;
        string _poNumber,_totalPOItems;
        bool _isrefreshing;
        public IncomingDetailModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string TotalPOItems { get => _totalPOItems; set => SetProperty(ref _totalPOItems, value); }
        public bool IsRefreshing { get => _isrefreshing; set => SetProperty(ref _isrefreshing, value); }

        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get;  }
        public OverviewDetailPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Tapped()
        {
            if(SelectedItem != null)
            {
                await PopupNavigation.Instance.PushAsync(new PartialDetailListPopupPage(SelectedItem));
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
            await QueryAll();
        }
        public async Task QueryAll()
        {
            IncomingDetailList.Clear();
            var totalpartialcqty = 0;
            var AllItemInThisPo = await localDbIncomingDetailService.GetList("PONumber", null, null);
            foreach(var item in AllItemInThisPo)
            {
                int[] g = { item.INCDetId };
                var e = await localDbIncomingParDetailService.GetList("PONumber&INCId", null, g);
                foreach(var ite in e)
                {
                    totalpartialcqty += ite.PartialCQTY;
                }
                if(item.Qty > totalpartialcqty)
                {
                    item.QTYStatus = "Short";
                }
                else if(item.Qty < totalpartialcqty)
                {
                    item.QTYStatus = "Over";
                }
                else if( item.Qty == totalpartialcqty)
                {
                    item.QTYStatus = "Ok";
                }
                item.Cqty = totalpartialcqty;
                totalpartialcqty = 0;
            }
            IncomingDetailList.AddRange(AllItemInThisPo);
            var n = IncomingDetailList.Count;
            TotalPOItems = n + " " + "Items";


        }


        private async Task Finalize()
        {
            var userId = Preferences.Get("PrefUserId", 0);
            IncomingHeaderModel e = new IncomingHeaderModel
            {
                FinalUserId = userId,
                PONumber = PONumber,
            };
            await localDbIncomingHeaderService.Update("PONumber",e);
            foreach(var item in IncomingDetailList)
            {
                await localDbIncomingDetailService // to be continue;
            }
        }
    }
}
