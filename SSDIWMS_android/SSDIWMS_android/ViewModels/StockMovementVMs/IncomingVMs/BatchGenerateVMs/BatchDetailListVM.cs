using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs
{
    public class BatchDetailListVM : ViewModelBase
    {
        ISMLBatchDetailsServices localDbBatchDetailService;
        
        public ObservableRangeCollection<BatchDetailsModel> BatchDetailList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public BatchDetailListVM()
        {
            BatchDetailList = new ObservableRangeCollection<BatchDetailsModel>();
            localDbBatchDetailService = DependencyService.Get<ISMLBatchDetailsServices>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);

        }
        private async Task PageRefresh()
        {
            BatchDetailList.Clear();
            var filter = new BatchDetailsModel
            {
                BatchCode = Preferences.Get("PrefBatchCode", string.Empty)
            };
            BatchDetailList.AddRange(await localDbBatchDetailService.GetList(filter, "BatchCode"));
        }
    }
}
