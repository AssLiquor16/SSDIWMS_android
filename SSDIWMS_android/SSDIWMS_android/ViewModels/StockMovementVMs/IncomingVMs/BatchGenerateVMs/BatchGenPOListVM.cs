using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.NotificationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs
{
    public class BatchGenPOListVM : ViewModelBase 
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        IToastNotifService notifService;
        ISMLBatchHeaderServices localDbBatchHeaderService;
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        public LiveTime livetime { get; } = new LiveTime();

        string _totalSelected;
        bool _isRefreshing, _generatebtnEnable;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public bool GeneratebtnEnable { get => _generatebtnEnable; set => SetProperty(ref _generatebtnEnable, value); }
        
        public string TotalSelected { get => _totalSelected; set => SetProperty(ref _totalSelected, value); }

        public ObservableRangeCollection<SelectedIncomingPartialHeaderModel> PartialModelRecievePOList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> SKUInAllSelectedPOList { get; set; }

        public AsyncCommand GenerateCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }


        public BatchGenPOListVM()
        {
            notifService = DependencyService.Get<IToastNotifService>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbBatchHeaderService = DependencyService.Get<ISMLBatchHeaderServices>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            SKUInAllSelectedPOList = new ObservableRangeCollection<IncomingDetailModel>();
            PartialModelRecievePOList = new ObservableRangeCollection<SelectedIncomingPartialHeaderModel>();
            GenerateCommand = new AsyncCommand(Generate);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
       public async Task TotalAllSelected()
        {
            await Task.Delay(1);
            var totCount = PartialModelRecievePOList.Where(x => x.IsSelected == true).Count();
            TotalSelected = $"Total selected Item(s): {totCount}";
            
        }
       private async Task Generate()
        {
            await notifService.LoadingProcess("Begin", "Generating...");
            if (PartialModelRecievePOList.Where(x => x.IsSelected == true).Count() != 0)
            {
                if (await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to generate batch code of the selected P.O?", "Yes", "No") == true)
                {
                    var selectedPO = PartialModelRecievePOList.Where(x => x.IsSelected == true).ToList();
                    var lastdig = "";
                    var remarks = "";
                    foreach(var po in selectedPO)
                    {
                        lastdig += po.ShipCode[po.ShipCode.Length - 1];
                        lastdig += po.Delcode[po.Delcode.Length - 1];
                        lastdig += po.BillDoc[po.BillDoc.Length - 1];
                        remarks += $"{po.PONumber}-"; // naa ni dash ha

                        
                    }
                    var batchcode = $"{DateTime.Today.Date.ToString("MMddy")}{lastdig}";
                    var bcontent = new BatchHeaderModel
                    {
                        BatchCode = batchcode,
                        UserCreated = Preferences.Get("PrefUserId", 0),
                        DateCreated = DateTime.Now,
                        Remarks = remarks,
                        TimesUpdated = 0,
                    };
                    var insertBc = await localDbBatchHeaderService.Insert(bcontent, "GenerateBatchCode");
                    foreach(var ipo in selectedPO)
                    {
                        string[] skufilter = { ipo.PONumber };
                        var skuPerPO = await localDbIncomingDetailService.GetList("PONumber", skufilter, null);
                        SKUInAllSelectedPOList.AddRange(skuPerPO);
                        foreach(var sku in SKUInAllSelectedPOList)
                        {
                            var batchdetContents = new BatchDetailsModel
                            {
                                BatchId = insertBc.BatchId,
                                BatchCode = insertBc.BatchCode,
                                ItemCode = sku.ItemCode,
                                ItemDesc = sku.ItemDesc,
                                Qty = sku.Cqty,
                                DateAdded = DateTime.Now,
                                TimesUpdated = 0
                            };
                        }
                        var icontent = new IncomingHeaderModel
                        {
                            INCId = ipo.INCId,
                            PONumber = ipo.PONumber,
                            BatchCode = insertBc.BatchCode,
                            TimesUpdated = ipo.TimesUpdated + 5,
                        };
                        await localDbIncomingHeaderService.Update("BatchCode", icontent);
                    }
                    await notifService.StaticToastNotif("Success", "BatchCode generate succesfully");
                    await PageRefresh();
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "No selected P.O", "Ok");
            }
            await notifService.LoadingProcess("End");
        }

        private async Task ColViewRefresh()
        {
            IsRefreshing = true;
            await PageRefresh();
            IsRefreshing = false;
        }
        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            SKUInAllSelectedPOList.Clear();
            PartialModelRecievePOList.Clear();
            var filters = new IncomingHeaderModel
            {
                WarehouseId = Preferences.Get("PrefUserWarehouseAssignedId", 0)
            };
            var datas = await localDbIncomingHeaderService.GetList("WhId/CurDate/RecievedIncStat",null ,null ,null , filters);
            datas = datas.Where(x => x.BatchCode == null).ToList();
            foreach(var data in datas)
            {
                var model = new SelectedIncomingPartialHeaderModel
                {
                    INCId = data.INCId,
                    PONumber = data.PONumber,
                    TimesUpdated = data.TimesUpdated,
                    INCStatus = data.INCstatus,
                    Delcode = data.DelCode,
                    ShipCode = data.ShipCode,
                    BillDoc = data.BillDoc,
                    BatchCode = data.BatchCode,
                    IsSelected = false
                };
                PartialModelRecievePOList.Add(model);
            }
            TotalSelected = $"Total selected Item(s): {PartialModelRecievePOList.Where(x=>x.IsSelected).Count()}";
        }
    }
}
