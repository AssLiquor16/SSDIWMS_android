using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.SummaryPopupModuleVMs
{
    public class SummaryPopupSubVM : ViewModelBase
    {
        GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();
        public LiveTime livetime { get; } = new LiveTime();
        string[] _pos;
        public string[] POS { get => _pos; set => SetProperty(ref _pos, value); }
        public ObservableRangeCollection<ConsolidationModel> OverallskuList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public SummaryPopupSubVM()
        {
            OverallskuList = new ObservableRangeCollection<ConsolidationModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        public async Task PageRefresh()
        {
            await livetime.LiveTimer();
            OverallskuList.Clear();
            foreach(var po in POS)
            {
                var incDetails = await dependencies.localDbIncomingDetailService.GetList("PONumber2",new string[] {po} , null );
                var groupedIncDets = incDetails.GroupBy(l => l.ItemCode).Select(cl => new ConsolidationModel
                {
                    ItemCode = cl.Key,
                    ItemDesc = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                    Qty = cl.Sum(c => c.Qty),
                    CQty = cl.Sum(c => c.Cqty)
                }).ToList();
                OverallskuList.AddRange(groupedIncDets);
            }
            foreach(var sku in OverallskuList)
            {
                if(sku.Qty > sku.CQty)
                {
                    sku.QTYStatus = "Short";
                    sku.Color = "Red";
                }
                else if(sku.Qty < sku.CQty)
                {
                    sku.QTYStatus = "Over";
                    sku.Color = "Red";
                }
                else if(sku.Qty == sku.CQty)
                {
                    sku.QTYStatus = "Ok";
                    sku.Color = "Green";
                }
            }

/*
            (l => l.ItemCode).Select(cl => new BatchDetailsModel
            {
                BatchId = insertBc.BatchLocalID,
                BatchCode = insertBc.BatchCode,
                ItemCode = cl.Key,
                ItemDesc = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                Qty = cl.Sum(c => c.Cqty),
                DateAdded = DateTime.Now,
                TimesUpdated = 0,
                DateSync = DateTime.Now
            }).ToList();*/

        }
    }

}
