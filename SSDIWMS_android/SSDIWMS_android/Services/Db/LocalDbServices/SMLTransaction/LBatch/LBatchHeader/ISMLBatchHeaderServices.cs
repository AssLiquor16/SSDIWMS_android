using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader
{
    public interface ISMLBatchHeaderServices
    {
        Task<IEnumerable<BatchHeaderModel>> GetList(BatchHeaderModel obj = null, string type = null);
        Task<BatchHeaderModel> GetFirstOrDefault(BatchHeaderModel obj = null, string type = null);
        Task<BatchHeaderModel> Insert(BatchHeaderModel obj = null, string type = null);
        Task<BatchHeaderModel> Update(BatchHeaderModel obj = null, string type = null);
        Task Remove(object obj, string type = null);
    }
}
