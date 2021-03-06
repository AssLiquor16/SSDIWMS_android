using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.SMTransactions;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.BatchGeneratePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.BatchGenerateVMs.BatchPopupVMs
{
    public class BatchGenPOListPopupVM : ViewModelBase
    {
        IMainTransactionSync mainTransactionSync;
        ISMLIncomingDetailServices localDbIncomingDetailService;
        IToastNotifService notifService;
        ISMLBatchHeaderServices localDbBatchHeaderService;
        ISMLBatchDetailsServices localDbbatchDetailsService;
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        public LiveTime livetime { get; } = new LiveTime();

        string _totalSelected;
        bool _isRefreshing, _generatebtnEnable;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        public bool GeneratebtnEnable { get => _generatebtnEnable; set => SetProperty(ref _generatebtnEnable, value); }

        public string TotalSelected { get => _totalSelected; set => SetProperty(ref _totalSelected, value); }

        public ObservableRangeCollection<SelectedIncomingPartialHeaderModel> PartialModelRecievePOList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> SKUInAllSelectedPOList { get; set; }
        public AsyncCommand SyncBatchCommand { get; }
        public AsyncCommand GenerateCommand { get; }
        public AsyncCommand ColViewRefreshCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }


        public BatchGenPOListPopupVM()
        {
            mainTransactionSync = DependencyService.Get<IMainTransactionSync>();
            notifService = DependencyService.Get<IToastNotifService>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbBatchHeaderService = DependencyService.Get<ISMLBatchHeaderServices>();
            localDbbatchDetailsService = DependencyService.Get<ISMLBatchDetailsServices>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            SKUInAllSelectedPOList = new ObservableRangeCollection<IncomingDetailModel>();
            PartialModelRecievePOList = new ObservableRangeCollection<SelectedIncomingPartialHeaderModel>();
            GenerateCommand = new AsyncCommand(Generate);
            SyncBatchCommand = new AsyncCommand(SyncBatch);
            ColViewRefreshCommand = new AsyncCommand(ColViewRefresh);
            PageRefreshCommand = new AsyncCommand(PageRefresh);

        }
        public async Task SyncBatch()
        {
            await notifService.LoadingProcess("Begin", "Syncing...");
            try
            {
                await mainTransactionSync.UpdateAllTransactions("Batch");
                await mainTransactionSync.UpdateSpecificTransactions("IncomingHeader");
                await notifService.StaticToastNotif("Success", "Batch synced.");
                await PageRefresh();
            }
            catch
            {
                await notifService.StaticToastNotif("Error","Cannot connect to server.");
            }
            await notifService.LoadingProcess("End");
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
                    foreach (var po in selectedPO)
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
                        DateSync = DateTime.Now,
                    };
                    var insertBc = await localDbBatchHeaderService.Insert(bcontent, "GenerateBatchCode");
                    foreach (var ipo in selectedPO)
                    {
                        string[] skufilter = { ipo.PONumber };
                        var skuPerPO = await localDbIncomingDetailService.GetList("PONumber2", skufilter, null);
                        SKUInAllSelectedPOList.AddRange(skuPerPO);
                        var icontent = new IncomingHeaderModel
                        {
                            INCId = ipo.INCId,
                            PONumber = ipo.PONumber,
                            BatchCode = insertBc.BatchCode,
                            TimesUpdated = ipo.TimesUpdated + 5,
                            DateSync = DateTime.Now,
                        };
                        await localDbIncomingHeaderService.Update("BatchCode", icontent);
                    }
                    var btdetcont = new BatchDetailsModel();
                    var groupbysku = SKUInAllSelectedPOList.GroupBy(l => l.ItemCode).Select(cl => new BatchDetailsModel
                    {
                        BatchId = insertBc.BatchLocalID,
                        BatchCode = insertBc.BatchCode,
                        ItemCode = cl.Key,
                        ItemDesc = cl.Where(x=>x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                        Qty = cl.Sum(c => c.Cqty),
                        DateAdded = DateTime.Now,
                        TimesUpdated = 0,
                        DateSync = DateTime.Now
                    }).ToList();
                    foreach(var gbs in groupbysku)
                    {
                        await localDbbatchDetailsService.Insert(gbs);
                    }
                    try
                    {
                        await mainTransactionSync.UpdateAllTransactions("Batch");
                        await mainTransactionSync.UpdateSpecificTransactions("IncomingHeader");
                        
                        await notifService.StaticToastNotif("Success", "Batch generated.");
                    }
                    catch(Exception ex)
                    {
                        await notifService.StaticToastNotif("Error", ex.Message);
                    }
                    MessagingCenter.Send(this, "RefreshIncomingHeaderList", string.Empty);
                    await PageRefresh();
                    await notifService.LoadingProcess("End");
                }
                else
                {
                    await notifService.LoadingProcess("End");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "No selected P.O", "Ok");
                await notifService.LoadingProcess("End");
            }
        }
        private async Task ColViewRefresh()
        {
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
            var datas = await localDbIncomingHeaderService.GetList("WhId/CurDate/RecievedIncStat&FinalizedIncStat", null, null, null, filters);
            foreach (var data in datas)
            {
                var allowbCodeGen = false;
                if(data.INCstatus == "Recieved" && data.BatchCode == null)
                {
                    allowbCodeGen = true;
                }
                else
                {
                    allowbCodeGen = false;
                }
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
                    IsSelected = false,
                    AllowGenBCode = allowbCodeGen
                };
                PartialModelRecievePOList.Add(model);
            }
            PartialModelRecievePOList.OrderBy(x=>x.AllowGenBCode).ToList();
            TotalSelected = $"Total selected Item(s): {PartialModelRecievePOList.Where(x => x.IsSelected).Count()}";
        }
        public async Task Close() => await PopupNavigation.Instance.PopAsync(true);
       

    }
}
