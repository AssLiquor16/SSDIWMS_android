using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchHeader
{
    public interface ISMSBatchHeaderServices
    {
        Task<IEnumerable<BatchHeaderModel>> GetList(object obj = null, string type = null);
        Task<BatchHeaderModel> GetFirstOrDefault(object obj = null, string type = null);
        Task<BatchHeaderModel> Insert(object obj = null, string type = null);
        Task<BatchHeaderModel> Update(object obj = null, string type = null);
    }
}
