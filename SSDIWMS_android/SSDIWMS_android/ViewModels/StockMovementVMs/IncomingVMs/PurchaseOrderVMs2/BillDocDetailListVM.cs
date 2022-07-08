using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.PurchaseOrderPages2.BillDocDetSubPages.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.PurchaseOrderVMs2
{
    public class BillDocDetailListVM : ViewModelBase
    {
        IncomingDetailModel _selectedItem;
        string _searchcode,_billdoc;
        bool _isRefreshing;
        public LiveTime livetime { get; } = new LiveTime();
        GlobalDependencyServices call = new GlobalDependencyServices();
        

        public IncomingDetailModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string BillDoc { get => _billdoc; set => SetProperty(ref _billdoc, value); }
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public string SearchCode
        {
            get => _searchcode;
            set
            {

                if (value == _searchcode)
                    return;
                _searchcode = value;
                OnPropertyChanged();
                Search(value);

            }
        }

        public ObservableRangeCollection<IncomingPartialDetailModel> MainBillDocPartialDetList { get; set; }
        public ObservableRangeCollection<IncomingPartialDetailModel> BillDocPartialDetList { get; set; }

        public AsyncCommand AddNavCommand { get; }
        public AsyncCommand SummaryNavCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ListRefreshCommand { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public BillDocDetailListVM()
        {

            MainBillDocPartialDetList = new ObservableRangeCollection<IncomingPartialDetailModel>();
            BillDocPartialDetList = new ObservableRangeCollection<IncomingPartialDetailModel>();

            AddNavCommand = new AsyncCommand(AddNav);
            SummaryNavCommand = new AsyncCommand(SummaryNav);
            TappedCommand = new AsyncCommand(Tapped);
            ListRefreshCommand = new AsyncCommand(ListRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task AddNav()
        {
            // navigate to add page
            await Shell.Current.GoToAsync($"{nameof(BillDocAddDetPage)}");
        }
        private async Task SummaryNav()
        {
            //popup summary page
        }
        private async Task Tapped()
        {
            if(SelectedItem != null)
            {
                // popup edit page
            }
            SelectedItem = null;
        }
        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            BillDoc = POStaticDatas.BillDoc;
            await ListRefresh();

        }
        private async Task ListRefresh()
        {
            MainBillDocPartialDetList.Clear(); BillDocPartialDetList.Clear();
            await call.notifService.LoadingProcess("Begin", "Loading...");
            try
            {
                var partialdetails = await call.serverDbIncomingPartialDetailService.NewGetList(new IncomingPartialDetailModel { BillDoc = POStaticDatas.BillDoc }, "BillDoc");
                MainBillDocPartialDetList.ReplaceRange(partialdetails);
                BillDocPartialDetList.ReplaceRange(MainBillDocPartialDetList);
            }
            catch
            {
                await call.notifService.StaticToastNotif("Error", "Unable to load list, cannot connect to server");
            }
            SearchCode = string.Empty;
            IsRefreshing = false;
            await call.notifService.LoadingProcess("End");
        }
        private void Search(string val)
        {
            val = val.ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(val))
            {
                BillDocPartialDetList.ReplaceRange(MainBillDocPartialDetList);
            }
            else
            {
                var result = MainBillDocPartialDetList.Where(x => x.ItemCode.Contains(val));
                BillDocPartialDetList.ReplaceRange(result);
            }
            
        }
        private async Task<List<IncomingDetailModel>> MergeDetails(List<IncomingDetailModel> objectlist)
        {
            await Task.Delay(100);
            var skus = objectlist.GroupBy(l => l.ItemCode).Select(cl => new IncomingDetailModel
            {
                INCDetId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().INCDetId,
                INCHeaderId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().INCHeaderId,
                ItemCode = cl.Key,
                ItemDesc = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                Qty = cl.Sum(x => x.Qty),
                Cqty = 0,
                UserId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().UserId,
                Amount = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().Amount,
                TimesUpdated = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().TimesUpdated,
                POHeaderNumber = string.Empty,
                DateSync = DateTime.MinValue
            }).ToList();
            return skus;
        }
    }
}
