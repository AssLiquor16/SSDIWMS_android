using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard
{
    public interface ISStockCardServices
    {
        Task<IEnumerable<StockCardsModel>> GetList(StockCardsModel obj = null, string type = null);
        Task<StockCardsModel> GetFirstOrDefault(StockCardsModel obj, string type = null);
        Task<StockCardsModel> Insert(StockCardsModel obj, string type = null);
        Task<StockCardsModel> Update(StockCardsModel obj, string type = null);
    }
}
