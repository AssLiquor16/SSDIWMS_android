using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
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
        public LiveTime liveTime { get; } = new LiveTime();
        ISMLBatchDetailsServices localDbBatchDetailService;
        bool _isRefreshing;
        BatchDetailsModel _selectedItem;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public BatchDetailsModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public ObservableRangeCollection<BatchDetailsModel> BatchDetailList { get; set; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public BatchDetailListVM()
        {
            BatchDetailList = new ObservableRangeCollection<BatchDetailsModel>();
            localDbBatchDetailService = DependencyService.Get<ISMLBatchDetailsServices>();
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

        }

        private async Task Tapped()
        {
            await Task.Delay(1);
            if(SelectedItem != null)
            {
                SelectedItem = null;
            }
        }

        private async Task ColViewRefresh()
        {
            IsRefreshing = true;
            await PageRefresh();
            IsRefreshing = false;
        }

        private async Task PageRefresh()
        {
            await liveTime.LiveTimer();
            BatchDetailList.Clear();
            var filter = new BatchDetailsModel
            {
                BatchCode = Preferences.Get("PrefBatchCode", string.Empty)
            };
            BatchDetailList.AddRange(await localDbBatchDetailService.GetList(filter, "BatchCode"));
        }
    }
}
