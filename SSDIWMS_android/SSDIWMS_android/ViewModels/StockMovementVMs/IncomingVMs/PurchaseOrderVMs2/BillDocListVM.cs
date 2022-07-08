using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.PurchaseOrderPages2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.PurchaseOrderVMs2
{
    public class BillDocListVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        GlobalDependencyServices call = new GlobalDependencyServices();
        string _searchcode;
        bool _isRefreshing;
        IncomingHeaderModel _selectedItem;
        public ObservableRangeCollection<IncomingHeaderModel> MainHeaderList { get; set; }
        public ObservableRangeCollection<IncomingHeaderModel> HeaderList { get; set; }

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

        public IncomingHeaderModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand ListRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }

        public BillDocListVM()
        {
            MainHeaderList = new ObservableRangeCollection<IncomingHeaderModel>();
            HeaderList = new ObservableRangeCollection<IncomingHeaderModel>();

            TappedCommand = new AsyncCommand(Tapped);
            ListRefreshCommand = new AsyncCommand(ListRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Tapped()
        {
            await call.notifService.LoadingProcess("Begin", "Loading...");
            if(SelectedItem != null)
            {
                try
                {
                    POStaticDatas.SetBillDoc(SelectedItem.BillDoc);
                    if (POStaticDatas.UserRole.Contains("Pick"))
                    {
                        if(SelectedItem.INCstatus == "Recieved")
                        {
                            // Show Popup Only.
                            return;
                        }
                        await Shell.Current.GoToAsync($"{nameof(BillDocDetailListPage)}");
                    }
                    else if (POStaticDatas.UserRole.Contains("Check"))
                    {
                        await Shell.Current.GoToAsync($"{nameof(BillDocDetailListPage)}");
                    }
                    
                }
                catch
                {
                    await call.notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
            }
            SelectedItem = null;
            await call.notifService.LoadingProcess("End");
        }
        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            await ListRefresh();    
        }
        private async Task ListRefresh()
        {
            await call.notifService.LoadingProcess("Begin", "Loading...");
            try
            {
                var defaultbilldoclist = await call.serverDbIncomingHeaderService.GetList("GetActDate", null, new int[] { POStaticDatas.WarehouseId }, null);
                var billdocList = await MergeHeader(defaultbilldoclist.ToList());
                var specifiedbyrole = new List<IncomingHeaderModel>();
                switch (POStaticDatas.UserRole)
                {
                    case "Pick":
                        var pickeritems = billdocList.Where(x => x.INCstatus == "Recieve" || x.INCstatus == "Finalized");
                        specifiedbyrole.AddRange(pickeritems); break;
                    case "Check":
                        var checkeritems = billdocList.Where(x => x.INCstatus == "Ongoing");
                        specifiedbyrole.AddRange(checkeritems); break;
                }
                MainHeaderList.ReplaceRange(specifiedbyrole);
                HeaderList.ReplaceRange(MainHeaderList);
                SearchCode = string.Empty;
            }
            catch
            {
                await call.notifService.StaticToastNotif("Error", "Unable to load list, cannot connect to server.");
            }
            IsRefreshing = false;
            await call.notifService.LoadingProcess("End");
        }
        private void Search(string val)
        {
            val = val.ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(val))
            {
                HeaderList.ReplaceRange(MainHeaderList);
            }
            else
            {
                var results = MainHeaderList.Where(x => x.BillDoc.Contains(val)).ToList();
                HeaderList.ReplaceRange(results);
            }
        }
        private async Task<List<IncomingHeaderModel>> MergeHeader(List<IncomingHeaderModel> objectlist)
        {
            await Task.Delay(100);
            var headers = objectlist.GroupBy(l => l.BillDoc).Select(cl => new IncomingHeaderModel
            {
                PONumber = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().PONumber,
                INCId = cl.Where(x=>x.BillDoc == cl.Key).FirstOrDefault().INCId,
                LoadDate = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().LoadDate,
                ShipCode = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().ShipCode,
                CstmrName = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().CstmrName,
                ReqDelDate = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().ReqDelDate,
                ShipNo = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().ShipNo,
                ShipLine = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().ShipLine,
                CVAN = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().CVAN,
                SalesDoc = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().SalesDoc,
                BillDoc = cl.Key,
                DelCode = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().DelCode,
                ReferenceDoc = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().ReferenceDoc,
                DelStatus = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().DelStatus,
                PlanDelSched = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().PlanDelSched,
                ActRecDate = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().ActRecDate,
                FinalDate = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().FinalDate,
                RecDate = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().RecDate,
                INCstatus = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().INCstatus,
                DateUploaded = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().DateUploaded,
                WarehouseId = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().WarehouseId,
                FinalUserId = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().FinalUserId,
                RecUserId = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().RecUserId,
                TimesUpdated = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().TimesUpdated,
                BatchCode = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().BatchCode,
                DateSync = cl.Where(x => x.BillDoc == cl.Key).FirstOrDefault().DateSync,

            }).ToList();
            return headers;
        }
    }
}
