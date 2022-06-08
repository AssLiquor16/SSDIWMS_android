using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs.BatchPopupVMs;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.BatchGeneratePages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.BatchGeneratePages.BatchPopupPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs
{
    public class BatchHeaderListVM : ViewModelBase
    {
        public LiveTime liveTime { get; } = new LiveTime();
        ISMLBatchHeaderServices localDbBatchHeaderService;

        bool _isRefreshing;
        BatchHeaderModel _selectedItem;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public BatchHeaderModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public ObservableRangeCollection<BatchHeaderModel> BatchHeaderList { get; set; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand GenerateNavCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public BatchHeaderListVM()
        {
            localDbBatchHeaderService = DependencyService.Get<ISMLBatchHeaderServices>();
            BatchHeaderList = new ObservableRangeCollection<BatchHeaderModel>();
            GenerateNavCommand = new AsyncCommand(GenerateNav);
            TappedCommand = new AsyncCommand(Tapped);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

            MessagingCenter.Subscribe<BatchGenPOListPopupVM>(this, "RefreshBatchList", async (page) =>
            {
                await PageRefresh();
            });
        }
        private async Task Tapped()
        {
            if(SelectedItem != null)
            {
                Preferences.Set("PrefBatchCode",SelectedItem.BatchCode);
                await Shell.Current.GoToAsync($"{nameof(BatchDetailListPage)}");
                SelectedItem = null;
            }
        }
        private async Task GenerateNav() => await PopupNavigation.Instance.PushAsync(new BatchGenPOListPopupPage());
        private async Task ColViewRefresh()
        {
            IsRefreshing = true;
            await PageRefresh();
            IsRefreshing = false;
        }
        private async Task PageRefresh()
        {
            await liveTime.LiveTimer();
            BatchHeaderList.Clear();
            BatchHeaderList.AddRange(await localDbBatchHeaderService.GetList());
        }
    }
}
