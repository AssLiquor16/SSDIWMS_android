using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Helpers
{
    public class GroupStockTransferHistory
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();

        public async Task<IEnumerable<StockTransferHistoryModel>> GroupStockCard(List<StockTransferHistoryModel> lists)
        {
            await Task.Delay(1);
            var e = lists.GroupBy(l => l.ItemCode).Select(cl => new StockTransferHistoryModel
            {
                MobileId = cl.FirstOrDefault().MobileId,
                ItemCode = cl.Key,
                ItemDesc = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                PalletCode = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().PalletCode,
                TransferType = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().TransferType,
                TransactionType = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().TransactionType,
                FromLocation = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().FromLocation,
                ToLocation = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ToLocation,
                DateTransact = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().DateTransact,
                UserId = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().UserId,
                TimesUpdated = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().TimesUpdated,
                DateSync = DateTime.Now,
                StockTransferLocalId = RandomStringGenerator.RandomString("STH"),
                Area = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().Area,
                Warehouse = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().Warehouse,
                Qty = cl.Sum(x => x.Qty)
            }).ToList();
            return e;
        }
    }
}
