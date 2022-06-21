using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard.SStockTransferHistories
{
    public interface ISStockTransferHistoriesServices
    {
        Task<IEnumerable<StockTransferHistoryModel>> GetList(StockTransferHistoryModel obj = null, string type = null);
        Task<StockTransferHistoryModel> GetFirstOrDefault(StockTransferHistoryModel obj, string type = null);
        Task<StockTransferHistoryModel> Insert(StockTransferHistoryModel obj, string type = null);
        Task<StockTransferHistoryModel> Update(StockTransferHistoryModel obj, string type = null);
    }
}
