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
        string _totSku;
        string[] _pos;
        public string TotSku { get => _totSku; set => SetProperty(ref _totSku, value); }
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
                foreach(var incdet in incDetails)
                {
                    OverallskuList.Add(new ConsolidationModel
                    {
                        ItemCode = incdet.ItemCode,
                        ItemDesc = incdet.ItemDesc,
                        Qty = incdet.Qty,
                        CQty = incdet.Cqty
                    });
                }
            }
            var conslist = OverallskuList.GroupBy(x => x.ItemCode).Select(xl => new ConsolidationModel
            {
                ItemCode = xl.Key,
                ItemDesc = xl.Where(x=>x.ItemCode == xl.Key).FirstOrDefault().ItemDesc,
                Qty = xl.Sum(b=>b.Qty),
                CQty = xl.Sum(c=>c.CQty)
            }).ToList();
            foreach(var sku in conslist)
            {
                if (sku.Qty > sku.CQty)
                {
                    sku.QTYStatus = "Short";
                    sku.Color = "Red";
                }
                else if (sku.Qty < sku.CQty)
                {
                    sku.QTYStatus = "Over";
                    sku.Color = "Red";
                }
                else if (sku.Qty == sku.CQty)
                {
                    sku.QTYStatus = "Ok";
                    sku.Color = "Green";
                }
            }
            OverallskuList.Clear();
            OverallskuList.AddRange(conslist);
            TotSku = $"{OverallskuList.Count()} Item(s);";
        }
    }

}
