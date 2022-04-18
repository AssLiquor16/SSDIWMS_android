using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs.IncomingDetailSubPopupModuleVMs
{
    public class PartialDetailListPopupVM : ViewModelBase
    {
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        IncomingDetailModel _passedItem;
        public IncomingDetailModel PassedItem { get => _passedItem; set => SetProperty(ref _passedItem, value); }
        public ObservableRangeCollection<IncomingPartialDetailModel> PartialDetailList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public PartialDetailListPopupVM()
        {
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            PartialDetailList = new ObservableRangeCollection<IncomingPartialDetailModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            if(PassedItem != null)
            {
                await GetList();
            }
        }

        private async Task GetList()
        {
            PartialDetailList.Clear();
            int[] e = { PassedItem.INCDetId };
            string[] f = { PassedItem.ItemCode };
            var partialdetret = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", f, e);
            //var partialdetret = await localDbIncomingParDetailService.GetList("PONumber&INCId", null,e);
            PartialDetailList.AddRange(partialdetret);
        }
    }
}
